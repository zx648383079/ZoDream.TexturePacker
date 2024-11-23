using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ZoDream.Shared.EditorInterface
{
    public interface IImageStyler
    {
        public string Name { get; }
        /// <summary>
        /// 计算结果
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public IImageStyle Compute(IImageLayer layer);
    }

    public interface IImageComputedStyler: IImageStyler
    {
        public int ActualWidth { get; }
        public int ActualHeight { get; }

        public void Clear();
    }

    public interface IImageStyleManager: IList<IImageStyler>
    {
        public bool TryGet(string name, [NotNullWhen(true)] out IImageStyler? styler);
    }
}
