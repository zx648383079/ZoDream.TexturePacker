namespace ZoDream.Plugin.Live2d
{
    internal class LD_ModelRoot
    {
        public LD_FileReference FileReferences { get; set; }
    }

    internal class LD_FileReference
    {
        public string Moc { get; set; }

        public string[] Textures { get; set; }
    }
}
