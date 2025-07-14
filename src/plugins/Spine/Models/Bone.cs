using System;
using System.Text.Json.Serialization;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Spine.Models
{
    internal class Bone : IReadOnlyStyle
    {
        public string Name { get; set; } = string.Empty;

        public float? Length { get; set; }

        public string Parent { get; set; } = string.Empty;
        [JsonPropertyName("rotation")]
        public float Rotate { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        public float ShearX { get; set; }
        public float ShearY { get; set; }
        public TransformMode Transform { get; set; }
        [JsonIgnore]
        public BoneRuntime Runtime { get; private set; }

        public Bone()
        {
            Runtime = new(this);
        }

    }
}
