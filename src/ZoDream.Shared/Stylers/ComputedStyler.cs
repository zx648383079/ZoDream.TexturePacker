using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Stylers
{
    public class ComputedStyler: Dictionary<string, IReadOnlyStyle>, IDictionary<string, IReadOnlyStyle>
    {

        public IReadOnlyStyle Compute(string name, IReadOnlyStyle style)
        {
            if (style is not ComputedStyle)
            {
                style = new ComputedStyle(style);
            }
            if (TryAdd(name, style))
            {
                return style;
            }
            this[name] = style;
            return style;
        }

        public IReadOnlyStyle Compute(string name, 
            IReadOnlyStyle style, string? parentStyle)
        {
            if (parentStyle is not null && TryGetValue(parentStyle, out var pStyle))
            {
                return Compute(name, style, pStyle);
            }
            return Compute(name, style);
        }

        public IReadOnlyStyle Compute(string name, IReadOnlyStyle style, IReadOnlyStyle? parentStyle)
        {
            if (parentStyle is null)
            {
                return Compute(name, style);
            }
            var res = new ComputedStyle(style, parentStyle);
            if (TryAdd(name, res))
            {
                return res;
            }
            this[name] = res;
            return res;
        }
    }
}
