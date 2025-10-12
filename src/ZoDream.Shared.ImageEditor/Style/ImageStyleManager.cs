using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ZoDream.Shared.EditorInterface;

namespace ZoDream.Shared.ImageEditor
{
    public class ImageStyleManager: List<IImageStyler>, IImageStyleManager
    {
        public const string DefaultName = "Normal";
        public const string RealName = "RealTime";

        public ImageStyleManager()
        {
            Add(Default);
            Add(Real);
        }

        public IImageStyler Default { get; private set; } = new NormalImageStyler();
        public IImageStyler Real { get; private set; } = new RealImageStyler();

        public bool TryGet(string name, [NotNullWhen(true)] out IImageStyler? styler)
        {
            foreach (var item in this)
            {
                if (item.Name == name)
                {
                    styler = item;
                    return true;
                }
            }
            styler = null;
            return false;
        }

        public void Dispose()
        {
            foreach (var item in this)
            {
                if (item is IDisposable d)
                {
                    d.Dispose();
                }
            }
        }
    }
}
