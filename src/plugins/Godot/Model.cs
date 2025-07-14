using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Godot
{
    internal class GD_Node(string name)
    {
        public string Name { get; set; } = name;

        public Dictionary<string, object> Properties { get; set; } = [];

        public bool HasInnerProperty => Properties.Where(item => item.Value is not GD_Type).Any();
    }

    internal class GD_Type(string name)
    {
        public string Name { get; set; } = name;

        public IList<object> Arguments { get; set; } = [];


        internal static GD_Type TryParse(string text)
        {
            var i = text.IndexOf('(');
            return new GD_Type(text[..i])
            {
                Arguments = text[(i + 1)..(text.Length - 1)].Split(',')
                .Select(i => {
                    return TryParseBasicType(i.Trim());
                }).ToArray()
            };
        }

        internal static object TryParseBasicType(string text)
        {
            if (text.StartsWith('"') && text.EndsWith('"'))
            {
                return text[1..(text.Length - 1)];
            }
            return text.Contains('.') ? float.Parse(text) : int.Parse(text);
        }
    }
}
