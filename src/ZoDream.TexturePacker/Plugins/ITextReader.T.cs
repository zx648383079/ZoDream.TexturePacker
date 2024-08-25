using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.TexturePacker.Plugins
{
    public interface ITextReader<T>
    {
        public bool Canable(string content);
        public T? Deserialize(string content, string fileName);
        public string Serialize(T data, string fileName);
    }
}
