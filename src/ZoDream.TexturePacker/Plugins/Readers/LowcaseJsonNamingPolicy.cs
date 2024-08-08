using System;
using System.Text.Json;

namespace ZoDream.TexturePacker.Plugins.Readers
{
    public class LowcaseJsonNamingPolicy : JsonNamingPolicy
    {

        private readonly int _offset = 'a' - 'A';
        public override string ConvertName(string name)
        {
            var code = name[0];
            return Convert.ToChar(code > 'a' ? (code - _offset) : (code + _offset)) + name[1..];
        }
    }
}
