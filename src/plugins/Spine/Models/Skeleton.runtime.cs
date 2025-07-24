namespace ZoDream.Plugin.Spine.Models
{
    public class SkeletonRuntime(SpineSkeletonController controller, SkeletonRoot skeleton)
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;

        public Skin Skin { get; set; }
    }
}
