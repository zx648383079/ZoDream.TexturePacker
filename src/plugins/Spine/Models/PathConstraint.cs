using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class PathConstraint : ConstraintBase
    {
        public string[] Bones { get; set; }
        /// <summary>
        /// Slot.Name
        /// </summary>
        public string Target { get; set; }
        public PositionMode PositionMode { get; set; }
        public SpacingMode SpacingMode { get; set; }
        public RotateMode RotateMode { get; set; }
        public float OffsetRotation { get; set; }
        public float Position { get; set; }
        public float Spacing { get; set; }
        public float RotateMix { get; set; }
        public float TranslateXMix { get; set; }
        public float TranslateYMix { get; set; }

        [JsonIgnore]
        public PathConstraintRuntime Runtime { get; internal set; }

    }
}
