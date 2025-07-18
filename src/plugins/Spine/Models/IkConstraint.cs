namespace ZoDream.Plugin.Spine.Models
{
    public class IkConstraint
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public string[] Bones { get; set; }
        /// <summary>
        /// Bone name
        /// </summary>
        public string Target { get; set; }

        public float Mix { get; set; }

        public int BendDirection { get; set; }
    }
}
