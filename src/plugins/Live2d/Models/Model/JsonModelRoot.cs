namespace ZoDream.Plugin.Live2d.Models
{
    internal class JsonModelRoot
    {
        public int Version { get; set; }
        public JsonModelFileReferences FileReferences { get; set; }
        public JsonModelGroup[] Groups { get; set; }

    }
}
