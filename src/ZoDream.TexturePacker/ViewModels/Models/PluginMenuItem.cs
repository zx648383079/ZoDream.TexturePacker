using System;
using System.Windows.Input;

namespace ZoDream.TexturePacker.ViewModels.Models
{
    public class PluginMenuItem(string group, string name, Type instanceType)
    {

        public const string ImportName = "IMPORT";
        public const string ExportName = "EXPORT";

        public PluginMenuItem(string name, Type instanceType)
            : this(string.Empty, name, instanceType)
        {
            
        }

        public string Name => name;

        public string Group => group;

        public Type InstanceType => instanceType;
        public ICommand? Command { get; private set; }
    }
}
