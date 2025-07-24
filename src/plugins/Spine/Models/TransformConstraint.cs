using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class TransformConstraint : ConstraintBase
    {
        public string[] Bones { get; set; }
        public string Target { get; set; }
        public float RotateMix { get; set; }
        public float TranslateXMix { get; set; }
        public float TranslateYMix { get; set; }
        public float ScaleXMix { get; set; }
        public float ScaleYMix { get; set; }
        public float ShearYMix { get; set; }

        public float OffsetRotation { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetScaleX { get; set; }
        public float OffsetScaleY { get; set; }
        public float OffsetShearY { get; set; }

        public bool Relative { get; set; }
        public bool Local { get; set; }

        [JsonIgnore]
        public TransformConstraintRuntime Runtime { get; internal set; }

    }
}
