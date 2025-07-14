namespace ZoDream.Plugin.Live2d.Models
{
    internal class JsonPhysicsSettings
    {
        public string Id { get; set; }
        public JsonPhysicsInput[] Input { get; set; }
        public JsonPhysicsOutput[] Output { get; set; }
        public JsonPhysicsVertex[] Vertices { get; set; }
        public JsonPhysicsNormalization Normalization { get; set; }
    }
}
