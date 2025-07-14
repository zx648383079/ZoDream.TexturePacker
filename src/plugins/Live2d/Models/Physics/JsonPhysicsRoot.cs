namespace ZoDream.Plugin.Live2d.Models
{
    internal class JsonPhysicsRoot
    {
        public int Version { get; set; }
        public JsonPhysicsMeta Meta { get; set; }
        public JsonPhysicsSettings[] PhysicsSettings { get; set; }

    }
}
