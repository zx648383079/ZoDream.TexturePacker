using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers.Unity
{
    public class JsonReader : BaseTextReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new UnityJsonNamingPolicy()
        };

        public override bool IsEnabled(string content)
        {
            if (content.Contains("\"SubTexture\"") && content.Contains("\"imagePath\""))
            {
                return true;
            }
            return content.Contains("\"items\"");
        }

        public override IEnumerable<SpriteLayerSection>? Deserialize(string content, string fileName)
        {
            var res = JsonSerializer.Deserialize<SpriteLayerSection>(content, _option);
            return res is null ? null : [res];
        }

        public override string Serialize(IEnumerable<SpriteLayerSection> data, string fileName)
        {
            return JsonSerializer.Serialize(data.First(), _option);
        }

    }
}
