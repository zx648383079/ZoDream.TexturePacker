using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class UnityJsonNamingPolicy : JsonNamingPolicy
    {
        public UnityJsonNamingPolicy()
        {
            var keys = new string[] { "SubTexture", "Items", "imagePath", "FileName" };
            for (var i = 0; i < keys.Length - 1; i+= 2)
            {
                _keyMap.Add(keys[i], keys[i + 1]);
                _keyMap.Add(keys[i + 1], keys[i]);
            }
        }

        private readonly Dictionary<string, string> _keyMap = [];
        private readonly int _offset = 'a' - 'A';
        public override string ConvertName(string name)
        {
            if (_keyMap.TryGetValue(name, out var key))
            {
                return key;
            }
            var code = name[0];
            return Convert.ToChar(code > 'a' ? (code - _offset) : (code + _offset)) + name[1..];
        }
    }
}
