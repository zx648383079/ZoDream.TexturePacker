using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class PhysicsConstraint : ConstraintBase
    {
        public string Bone { get; internal set; }
        public float X { get; internal set; }
        public float Y { get; internal set; }
        public float Rotate { get; internal set; }
        public float ScaleX { get; internal set; }
        public float ShearX { get; internal set; }
        public float Limit { get; internal set; }
        public float Step { get; internal set; }
        public float Inertia { get; internal set; }
        public float Strength { get; internal set; }
        public float Damping { get; internal set; }
        public float MassInverse { get; internal set; }
        public float Wind { get; internal set; }
        public float Gravity { get; internal set; }
        public float Mix { get; internal set; }


        [JsonIgnore]
        public PhysicsConstraintRuntime Runtime { get; internal set; }

    }
}
