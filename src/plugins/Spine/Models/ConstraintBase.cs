using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public abstract class ConstraintBase
    {
        public string Name { get; set; }

        public int Order { get; set; }
        [JsonIgnore]
        public bool SkinRequired { get; set; }
    }
}
