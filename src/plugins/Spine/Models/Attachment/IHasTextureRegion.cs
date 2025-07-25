namespace ZoDream.Plugin.Spine.Models
{
    public interface IHasTextureRegion
    {
        public string Path { get; }

        public TextureRegion Region { get; set; }

        public void UpdateRegion();
    }
}
