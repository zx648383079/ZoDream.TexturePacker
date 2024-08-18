﻿using SkiaSharp;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.TexturePacker.Plugins.Bitmaps.Pvr;

namespace ZoDream.TexturePacker.Plugins.Readers.TexturePacker
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
            return buffer[0] == 0x1F && buffer[1] == 0x8B;
        }

        public Task<SKBitmap?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                using var fs = File.OpenRead(fileName);
                return Read(fs);
            });
        }

        private SKBitmap? Read(Stream input)
        {
            byte[] buffer;
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
        private SKBitmap? DecodeGZIP(Stream input)
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
        private SKBitmap? DecodeCCZP(Stream input)
        {
            input.Seek(12, SeekOrigin.Begin);
            var buffer = new byte[input.Length - 12];
            input.Read(buffer, 0, buffer.Length);
            var key = new PvrKey("");
            key.Decrypt(ref buffer);
            return DecodeCCZ(buffer);
        }

        private SKBitmap? DecodeCCZ(Stream input)
        {
            input.Seek(12, SeekOrigin.Begin);
            var buffer = new byte[input.Length - 12];
            input.Read(buffer, 0, buffer.Length);
            return DecodeCCZ(buffer);
        }

        private SKBitmap? DecodeCCZ(byte[] buffer)
        {
            buffer = Decompress(buffer.Skip(4).ToArray());
            return DecodePVR(buffer);
        }
        private SKBitmap? DecodePVR(Stream input)
        {
            var buffer = new byte[input.Length];
            input.Read(buffer, 0, buffer.Length);
            return DecodePVR(buffer);
        }
        private SKBitmap? DecodePVR(byte[] buffer)
        {
            return PvtImage.Decode(buffer);
        }

        public async Task<SKBitmap?> ReadAsync(IStorageFile file)
        {
            using var fs = await file.OpenReadAsync();
            return Read(fs.AsStreamForRead());
        }

        public Task WriteAsync(string fileName, SKBitmap data)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(IStorageFile file, SKBitmap data)
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