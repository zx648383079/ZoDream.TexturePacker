using SkiaSharp;
using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    internal class Slot
    {
        public string Attachment { get; set; } = string.Empty;

        public string Bone { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Index { get; set; }
        public SKColor? Color { get; set; }
        public SKColor? DarkColor { get; set; }
        [JsonPropertyName("blend")]
        public BlendMode BlendMode { get; set; }

        [JsonIgnore]
        public SlotRuntime Runtime { get; private set; } = new();
    }
}
