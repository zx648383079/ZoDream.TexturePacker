using System;
using System.Text.Json;

namespace ZoDream.Shared.IO
{
    public class LowCaseJsonNamingPolicy : JsonNamingPolicy
    {

        private readonly int _offset = 'a' - 'A';
        public override string ConvertName(string name)
        {
            var code = name[0];
            return Convert.ToChar(code > 'a' ? (code - _offset) : (code + _offset)) + name[1..];
        }
    }
}
