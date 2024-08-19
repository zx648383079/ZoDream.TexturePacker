using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using ZoDream.Shared.Storage;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Godot
{
    public class TresReader : IPluginReader, ITextReader
    {
        public bool Canable(string content)
        {
            return content.StartsWith("[gd_resource") && content.Contains("uid=\"");
        }
        public SpriteLayerSection? Deserialize(string content)
        {
            return Deserialize(content, string.Empty);
        }
        public SpriteLayerSection? Deserialize(string content, string root)
        {
            var data = GodotSerializer.Deserialize(content);
            var fileName = GetImagePath(data);
            var res = new SpriteLayerSection()
            {
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
            return res;
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

        public async Task<SpriteLayerSection?> ReadAsync(string fileName)
        {
            var text = await LocationStorage.ReadAsync(fileName);
            return Deserialize(text, GodotSerializer.GetGodotProjectRoot(fileName));
        }

        public async Task<SpriteLayerSection?> ReadAsync(IStorageFile file)
        {
            var text = await FileIO.ReadTextAsync(file);
            return Deserialize(text, GodotSerializer.GetGodotProjectRoot(file.Path));
        }
        public string Serialize(SpriteLayerSection data)
        {
            return Serialize(data, string.Empty);
        }
        public string Serialize(SpriteLayerSection data, string root)
        {
            var fileName = GodotSerializer.GetResourcePath(root, data.FileName);
            var uid = GodotSerializer.GenerateUID();
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
                           {"uid", uid},
                           {"path", fileName },
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

                File.WriteAllText(Path.Combine(root, item.Name + ".tres"), res);
            }
            return string.Empty;
        }

        public async Task WriteAsync(string fileName, SpriteLayerSection data)
        {
            // await LocationStorage.WriteAsync(fileName, Serialize(data, GodotSerializer.GetGodotProjectRoot(fileName)));
        }

        public async Task WriteAsync(IStorageFile file, SpriteLayerSection data)
        {
            // await FileIO.WriteTextAsync(file, Serialize(data, GodotSerializer.GetGodotProjectRoot(file.Path)), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }
    }
}
