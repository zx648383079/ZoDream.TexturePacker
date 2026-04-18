using System.Collections.Generic;
using ZoDream.Shared.Interfaces;
using ZoDream.TexturePacker.ViewModels.Models;

namespace ZoDream.TexturePacker.Plugins
{
    internal class PluginCollection : Dictionary<string, List<PluginMenuItem>>, IPluginCollection
    {
        public void Add<T>(string group, string label)
        {
            group = group.ToUpper();
            var item = new PluginMenuItem(group, label, typeof(T));
            if (TryGetValue(group, out var items))
            {
                items.Add(item);
                return;
            }
            items =
            [
                item
            ];
            Add(group, items);
        }

        public PluginMenuItem[] Get(string group)
        {
            group = group.ToUpper();
            if (TryGetValue(group, out var items))
            {
                return [..items];
            }
            return [];
        }

        public string GetGroupName(string group)
        {
            group = group.ToUpper();
            return group switch
            {
                PluginMenuItem.ImportName => "导入",
                PluginMenuItem.ExportName => "导出",
                _ => group,
            };
        }
    }
}
