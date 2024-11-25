using ZoDream.Shared.Drawing;

namespace ZoDream.Shared.Models
{
    public class ImageSection(string fileName, IImageData source, params string[] metaItems)
    {
        public string FileName { get; private set; } = fileName;

        public IImageData Source { get; private set; } = source;

        public string[] MetaItems { get; set; } = metaItems;
    }
}
