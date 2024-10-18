using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.IO;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Godot
{
    public class TresReader : IPluginReader, ITextReader
    {
        public bool IsEnabled(string content)
        {
            return content.StartsWith("[gd_resource") && content.Contains("uid=\"");
        }
        public IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            return Deserialize(content, string.Empty);
        }
        public IEnumerable<SpriteLayerSection>? DeserializeWithRoot(string content, string root)
        {
            var data = GodotSerializer.Deserialize(content);
            var fileName = GetImagePath(data);
            var res = new SpriteLayerSection()
            {
                UseCustomName = true,
                Name = Path.GetFileNameWithoutExtension(fileName),
                FileName = GodotSerializer.Combine(root, fileName),
            };
            foreach (var item in data)
            {
                if (item.Name == "resource")
                {
                    var bound = item.Properties["region"] as GD_Type;
                    res.Items.Add(new()
                    {
                        Name = (item.Properties["atlas"] as GD_Type).Arguments[0].ToString(),
                        X = Convert.ToInt32(bound.Arguments[0]),
                        Y = Convert.ToInt32(bound.Arguments[1]),
                        Width = Convert.ToInt32(bound.Arguments[2]),
                        Height = Convert.ToInt32(bound.Arguments[3]),
                    });
                }
            }
            return [res];
        }

        private string GetImagePath(IEnumerable<GD_Node> items)
        {
            foreach (var item in items)
            {
                if (item.Name == "ext_resource" && 
                    item.Properties["type"].ToString() == "Texture2D")
                {
                    return item.Properties["path"].ToString()?? string.Empty;
                }
            }
            return string.Empty;
        }

        public async Task<IEnumerable<SpriteLayerSection>?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return DeserializeWithRoot(text, GodotSerializer.GetGodotProjectRoot(fileName));
        }

        public async Task<IEnumerable<SpriteLayerSection>?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text, GodotSerializer.GetGodotProjectRoot(file.Path));
        }
        public string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            return Serialize(data, string.Empty);
        }
        public string SerializeWithRoot(IEnumerable<SpriteLayerSection> data, string root)
        {
            foreach (var item in data)
            {
                return SerializeWithRoot(data, root);
            }
            return string.Empty;
        }
        public string SerializeWithRoot(SpriteLayerSection data, string root)
        {
            var (imageUid, imageRes) = LoadImageImport(data.FileName, root);
            var spriteFolder = Path.Combine(Path.GetDirectoryName(data.FileName), 
                Path.GetFileNameWithoutExtension(data.FileName) + ".sprites");
            if (!Directory.Exists(spriteFolder))
            {
                Directory.CreateDirectory(spriteFolder);
            }
            foreach (var item in data.Items)
            {
                var id = GodotSerializer.GenerateID();
                var res = GodotSerializer.Serialize([
                    new GD_Node("gd_resource") {
                       Properties = new() {
                           {"type", "AtlasTexture" },
                           {"load_steps", 2},
                           {"format", 3},
                           {"uid", GodotSerializer.GenerateUID()},
                       }
                    },
                    new GD_Node("ext_resource") {
                       Properties = new() {
                           {"type", "Texture2D" },
                           {"uid", imageUid},
                           {"path", imageRes },
                           {"id",  id}
                       }
                    },
                    new GD_Node("resource") {
                       Properties = new() {
                           {"atlas", new GD_Type("ExtResource") { Arguments = [id] } },
                           {"region", new GD_Type("Rect2"){ Arguments = [item.X, item.Y, item.Width, item.Height] } },
                       }
                    }
                ]);

                File.WriteAllText(Path.Combine(spriteFolder, item.Name + ".tres"), res);
            }
            return string.Empty;
        }

        public async Task WriteAsync(string fileName, IEnumerable<SpriteLayerSection> data)
        {
            // await LocationStorage.WriteAsync(fileName, Serialize(data, GodotSerializer.GetGodotProjectRoot(fileName)));
        }

        public async Task WriteAsync(IStorageFile file, IEnumerable<SpriteLayerSection> data)
        {
            // await FileIO.WriteTextAsync(file, Serialize(data, GodotSerializer.GetGodotProjectRoot(file.Path)), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }


        private (string, string) CreateImageImport(string fileName, string root)
        {
            var uid = GodotSerializer.GenerateUID();
            var relativeFile = GodotSerializer.GetResourcePath(root, fileName);
            var sb = new StringBuilder();
            sb.AppendLine("[remap]")
                .AppendLine("importer=\"texture\"")
                .AppendLine("type=\"CompressedTexture2D\"")
                .AppendLine($"uid=\"{uid}\"")
                .AppendLine("metadata={")
                .AppendLine("\"vram_texture\": false")
                .AppendLine("}")
                .AppendLine()
                .Append("[deps]")
                .AppendLine($"source_file=\"{relativeFile}\"")
                .AppendLine()
                .AppendLine("[params]")
                .AppendLine("compress/mode=3")
                ;
            File.WriteAllText(fileName + ".import", sb.ToString());
            return (uid, relativeFile);
        }

        private (string, string) LoadImageImport(string fileName, string root)
        {
            var target = fileName + ".import";
            if (!File.Exists(target)) 
            {
                return CreateImageImport(fileName, root);
            }
            var content = File.ReadAllText(fileName + ".import");
            var uid = ReaderHelper.MatchWithRange(content, "uid=\"", "\"");
            var relativeFile = ReaderHelper.MatchWithRange(content, "source_file=\"", "\"");
            return (uid, relativeFile);
        }

        
    }
}
