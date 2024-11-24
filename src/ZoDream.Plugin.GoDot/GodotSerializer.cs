using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Godot
{
    public class GodotSerializer
    {
        public static string GenerateUID()
        {
            return "uid://" + Random(13);
        }
        public static string GenerateID()
        {
            return "1_" + Random(5);
        }

        public static string Random(int length)
        {
            var rand = new Random();
            var code = string.Empty;
            for (var i = 0; i < length; i++)
            {
                code += Letter(rand.Next(0, 36));
            }
            return code;
        }

        private static char Letter(int index)
        {
            if (index < 10)
            {
                return Convert.ToChar(48 + index);
            }
            if (index < 36)
            {
                return Convert.ToChar(87 + index);
            }
            if (index < 62)
            {
                return Convert.ToChar(29 + index);
            }
            return '-';
        }
        public static string Combine(string root, string path)
        {
            
            if (!string.IsNullOrEmpty(root) && path.StartsWith("res://"))
            {
                return Path.Combine(root, path[6..]);
            }
            return Path.GetFullPath(path);
        }

        public static string GetResourcePath(string root, string path)
        {
            if (string.IsNullOrEmpty(root))
            {
                return "res://" + Path.GetFileName(path);
            }
            return "res://" + Path.GetRelativePath(root, path);
        }

        public static string GetGodotProjectRoot(string fileName)
        {
            fileName = Path.GetFullPath(fileName);
            if (File.Exists(fileName))
            {
                fileName = Path.GetDirectoryName(fileName);
            }
            while (!string.IsNullOrWhiteSpace(fileName))
            {
                if (File.Exists(Path.Combine(fileName, "project.godot")))
                {
                    return fileName;
                }
                fileName = Path.GetDirectoryName(fileName);
            }
            return string.Empty;
        }

        internal static IEnumerable<GD_Node> Deserialize(string content)
        {
            var lines = content.Split('\n');
            var items = new List<GD_Node>();
            var last = new GD_Node(string.Empty);
            foreach (var li in lines)
            {
                var line = li.Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                string[] args;
                if (line.StartsWith('['))
                {
                    args = line[1..(line.Length - 1)].Split(' ');
                    last = new GD_Node(args[0]);
                    items.Add(last);
                    for (var i = 1; i < args.Length; i++)
                    {
                        var temp = args[i].Trim().Split("=", 2);
                        last.Properties.Add(temp[0], GD_Type.TryParseBasicType(temp[1]));
                    }
                    continue;
                }
                args = line.Split("=", 2);
                last.Properties.Add(args[0].Trim(), GD_Type.TryParse(args[1].Trim()));
            }
            return [.. items];
        }

        internal static string Serialize(IEnumerable<GD_Node> data)
        {
            var sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append('[')
                    .Append(item.Name);
                foreach (var it in item.Properties)
                {
                    if (it.Value is GD_Type)
                    {
                        continue;
                    }
                    sb.Append(' ').Append(it.Key);
                    if (it.Value is string)
                    {
                        sb.Append('"').Append(it.Value).Append('"');
                        continue;
                    }
                    sb.Append(it.Value);
                }
                sb.Append("]\n");
                foreach (var it in item.Properties)
                {
                    if (it.Value is not GD_Type o)
                    {
                        continue;
                    }
                    sb.Append(it.Key).Append(" = ").Append(o.Name).Append('(');
                    var j = 0;
                    foreach (var i in o.Arguments)
                    {
                        j++;
                        if (j > 1)
                        {
                            sb.Append(", ");
                        }
                        if (i is string)
                        {
                            sb.Append('"').Append(i).Append('"');
                            continue;
                        }
                        sb.Append(i);
                    }
                    sb.Append(")\n");
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }
    }
}
