using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Live2d
{
    public class MocReader : IPluginReader
    {
        public Task<IEnumerable<SpriteLayerSection>?> ReadAsync(string fileName)
        {
            return Task.Factory.StartNew(() => {
                var jsonFileName = GetModelJsonFile(fileName);
                if (jsonFileName is null)
                {
                    return null;
                }
                return Read(fileName, MocJsonReader.LoadTexture(jsonFileName));
            });
        }


        private string GetModelJsonFile(string fileName)
        {
            var baseFile = fileName.Substring(0, fileName.Length -
                Path.GetExtension(fileName).Length);
            fileName = baseFile + ".model3.json";
            if (File.Exists(fileName))
            {
                return fileName;
            }
            fileName = baseFile + ".model.json";
            if (File.Exists(fileName))
            {
                return fileName;
            }
            fileName = baseFile + ".json";
            return File.Exists(fileName) ? fileName : string.Empty;
        }

        public static IEnumerable<SpriteLayerSection>? Read(string fileName, string[] textureItems)
        {
            if (textureItems.Length == 0 || !File.Exists(fileName))
            {
                return null;
            }
            using var fs = File.OpenRead(fileName);
            return Read(fs, textureItems);
        }
        private static IEnumerable<SpriteLayerSection>? Read(Stream input, string[] textureItems)
        {
            var buffer = new byte[4];
            input.Read(buffer, 0, buffer.Length);
            if (buffer[0] != 0x4D || buffer[1] != 0x4F || buffer[2] != 0x43 ||
                buffer[3] != 0x33)
            {
                return null;
            }
            input.Seek(0x04, SeekOrigin.Begin);
            var version = input.ReadByte(); // 版本

            input.Seek(0x40, SeekOrigin.Begin);
            var reader = new BinaryReader(input);
            var ptrPos = reader.ReadUInt32();// + (version >= 5 ? 256 : 128);
            var lastPos = input.Position;
            input.Seek(ptrPos, SeekOrigin.Begin);
            Debug.WriteLine($"pos: 0x{input.Position:X}");
            var counter = StructConvert.ToStruct<MocCountInfoTable>(input);
            if (version >= 4)
            {
                input.Seek(36 + version >= 5 ? 12 : 0, SeekOrigin.Current);
            }
            input.Seek(lastPos, SeekOrigin.Begin);
            // CanvasInfo
            input.Seek(4, SeekOrigin.Current);
            // part
            input.Seek(4 + 7 * 4, SeekOrigin.Current);
            // Deformers
            input.Seek(4 + 8 * 4, SeekOrigin.Current);
            // WarpDeformers
            input.Seek(6 * 4, SeekOrigin.Current);
            // RotationDeformers
            input.Seek(4 * 4, SeekOrigin.Current);

            #region ArtMeshes
            input.Seek(16 + 8 * 4, SeekOrigin.Current);

            Debug.WriteLine($"pos: 0x{input.Position:X}");
            var meshCount = counter.ArtMeshes;

            ptrPos = reader.ReadUInt32(); // uv 对应的图片编号位置
            lastPos = input.Position;
            input.Seek(ptrPos, SeekOrigin.Begin);
            // 获取图片对应的编号
            var textureNo = new uint[meshCount];
            for (var i = 0; i < meshCount; i++)
            {
                textureNo[i] = reader.ReadUInt32();
            }
            input.Seek(lastPos + 4, SeekOrigin.Begin);
            // uv 对应的个数位置
            var uvCountItems = ReadPart(reader, meshCount, reader => reader.ReadInt32());
            // 获取在UV列表中的开始位置
            var uvOffsetItems = ReadPart(reader, meshCount, reader => reader.ReadUInt32());

            input.Seek(4 * 4, SeekOrigin.Current);
            #endregion

            // Parameters
            input.Seek(4 + 8 * 4, SeekOrigin.Current);
            
            // PartKeyforms
            input.Seek( 4, SeekOrigin.Current);
            // WarpDeformerKeyforms
            input.Seek(2 * 4, SeekOrigin.Current);
            // RotationDeformerKeyforms
            input.Seek(7 * 4, SeekOrigin.Current);
            // ArtMeshKeyforms
            input.Seek(3 * 4, SeekOrigin.Current);
            // KeyformPositions
            input.Seek(4, SeekOrigin.Current);
            // ParameterBindingIndices
            input.Seek(4, SeekOrigin.Current);
            // KeyformBindings
            input.Seek(2 * 4, SeekOrigin.Current);
            // ParameterBindings
            input.Seek(2 * 4, SeekOrigin.Current);
            // Keys
            input.Seek(4, SeekOrigin.Current);

            // UVS
            //input.Seek(4, SeekOrigin.Current);

            Debug.WriteLine($"pos: 0x{input.Position:X}");
            ptrPos = reader.ReadUInt32();
            lastPos = input.Position;
            var uvPos = ptrPos;
            var uvItems = new Vector2[meshCount][];
            // input.Seek(0x82c40, SeekOrigin.Begin);
            for (var i = 0; i < meshCount; i++)
            {
                input.Seek(uvPos + uvOffsetItems[i] * 4, SeekOrigin.Begin);
                //input.Seek(uvOffset[i], SeekOrigin.Begin);
                var uv = new Vector2[uvCountItems[i]];
                for (var j = 0; j < uv.Length; j++)
                {
                    uv[j] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                }
                uvItems[i] = uv;
            }


            var items = textureItems.Select((texture, n) => {
                var block = new SpriteLayerSection()
                {
                    Name = Path.GetFileName(texture),
                    FileName = texture,
                };
                for (var i = 0; i < textureNo.Length; i++)
                {
                    if (textureNo[i] != n)
                    {
                        continue;
                    }
                    block.Items.Add(new SpriteUvLayer()
                    {
                        Name = i.ToString(),
                        VertexItems = uvItems[i]
                    });
                }
                return block;
            });
            return items;
        }

        private static T[] ReadPart<T>(BinaryReader reader, uint length, Func<BinaryReader, T> fn)
        {
            var ptrPos = reader.ReadUInt32(); // uv 对应的个数位置
            var lastPos = reader.BaseStream.Position;
            reader.BaseStream.Seek(ptrPos, SeekOrigin.Begin);
            var items = new T[length];
            for (var i = 0; i < length; i++)
            {
                // 获取在 UV 列表中的个数
                items[i] = fn.Invoke(reader);
            }
            reader.BaseStream.Seek(lastPos, SeekOrigin.Begin);
            return items;
        }

        public Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            throw new NotImplementedException();
        }

    }
}
