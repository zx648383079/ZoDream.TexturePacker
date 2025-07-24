using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class IkConstraint : ConstraintBase
    {

        public string[] Bones { get; set; }
        /// <summary>
        /// Bone name
        /// </summary>
        public string Target { get; set; }

        public float Mix { get; set; }

        public float Softness { get; set; }
        public bool Compress { get; set; }
        public bool Stretch { get; set; }
        public bool Uniform { get; set; }

        public int BendDirection { get; set; }

        [JsonIgnore]
        public IkConstraintRuntime Runtime { get; internal set; }

    }
}
