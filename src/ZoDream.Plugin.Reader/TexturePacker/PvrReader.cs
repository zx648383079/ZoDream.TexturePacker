using SkiaSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using ZoDream.Shared.Drawing;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Readers.TexturePacker
{
    /// <summary>
    /// pvr,pvr.gz,.pvr.ccz
    /// </summary>
    public class PvrReader : IImageReader
    {
        public bool IsSupport(Stream input)
        {
            return GetFileType(input) != PVRFileType.Unknown;
        }

        private PVRFileType GetFileType(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[4];
            input.Read(buffer, 0, buffer.Length);
            if (buffer[0] == 0x1F && buffer[1] == 0x8B)
            {
                return PVRFileType.GZIP;
            }
            if (buffer[0] == 'P' &&  buffer[1] == 'V' && buffer[2] == 'R')
            {
                return PVRFileType.PVR;
            }
            
            if (buffer[0] != 'C' || buffer[1] != 'C' || buffer[2] != 'Z')
            {
                return PVRFileType.Unknown;
            }
            // buffer[3] == '!'
            return buffer[3] == 'p' ? PVRFileType.CCZ_P : PVRFileType.CCZ;
        }

        public bool IsProtection(Stream input)
        {
            input.Seek(3, SeekOrigin.Begin);
            return input.ReadByte() == 'p';
        }

        public bool IsGzip(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[2];
            stream.Read(buffer, 0, buffer.Length);
            return buffer.SequenceEqual(new byte[] { 0x1F, 0x8B });
        }

        public Task<IImageData?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                using var fs = File.OpenRead(fileName);
                return Read(fs);
            });
        }

        private IImageData? Read(Stream input)
        {
            var type = GetFileType(input);
            return type switch
            {
                PVRFileType.GZIP => DecodeGZIP(input),
                PVRFileType.PVR => DecodePVR(input),
                PVRFileType.CCZ => DecodeCCZ(input),
                PVRFileType.CCZ_P => DecodeCCZP(input),
                _ => null,
            };
        }
        private IImageData? DecodeGZIP(Stream input)
        {
            var reader = new GZipStream(input, CompressionMode.Decompress);
            var working = new byte[1024];
            using var output = new MemoryStream();
            int n;
            while ((n = reader.Read(working, 0, working.Length)) != 0)
            {
                output.Write(working, 0, n);
            }
            return DecodePVR(output.ToArray());
        }
        private IImageData? DecodeCCZP(Stream input)
        {
            input.Seek(12, SeekOrigin.Begin);
            var buffer = new byte[input.Length - 12];
            input.Read(buffer, 0, buffer.Length);
            var key = new PvrKey("");
            key.Decrypt(ref buffer);
            return DecodeCCZ(buffer);
        }

        private IImageData? DecodeCCZ(Stream input)
        {
            input.Seek(12, SeekOrigin.Begin);
            var buffer = new byte[input.Length - 12];
            input.Read(buffer, 0, buffer.Length);
            return DecodeCCZ(buffer);
        }

        private IImageData? DecodeCCZ(byte[] buffer)
        {
            buffer = Decompress(buffer.Skip(4).ToArray());
            return DecodePVR(buffer);
        }
        private IImageData? DecodePVR(Stream input)
        {
            var buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            return DecodePVR(buffer);
        }
        private IImageData? DecodePVR(byte[] buffer)
        {
            return PvtImage.Decode(buffer);
        }

        public Task WriteAsync(string fileName, IImageData data)
        {
            throw new NotImplementedException();
        }

        private byte[] Decompress(byte[] buffer) 
        {
            using var ms = new MemoryStream(buffer);
            var reader = new ZLibStream(ms, CompressionMode.Decompress);
            var working = new byte[1024];
            using var output = new MemoryStream();
            int n;
            while ((n = reader.Read(working, 0, working.Length)) != 0)
            {
                output.Write(working, 0, n);
            }
            return output.ToArray();
        }

        enum PVRFileType
        {
            Unknown = 0,
            PVR,
            CCZ,
            CCZ_P,
            GZIP
        }
    }
}
