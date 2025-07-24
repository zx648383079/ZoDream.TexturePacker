using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class SkeletonHeader
    {
        public string Hash { get; set; }

        public float Height { get; set; }

        public float Width { get; set; }
        [JsonPropertyName("images")]
        public string ImagesPath { get; set; }
        [JsonPropertyName("spine")]
        public string Version { get; set; }
        public float Fps { get; set; } = 30;
        public float X { get; internal set; }
        public float Y { get; internal set; }
        public float ReferenceScale { get; internal set; } = 100;
        [JsonPropertyName("audio")]
        public string AudioPath { get; internal set; }
    }
}
