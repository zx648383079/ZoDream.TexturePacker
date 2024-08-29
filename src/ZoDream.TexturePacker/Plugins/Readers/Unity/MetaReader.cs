﻿using System;
using System.Collections.Generic;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class MetaReader : BaseTextReader<string>, IFileMetaReader
    {
        public override bool Canable(string content)
        {
            return content.Contains("guid:");
        }

        protected override string GetFullPath(string fileName)
        {
            if (fileName.EndsWith(".meta"))
            {
                return fileName;
            }
            return fileName + ".meta";
        }

        public override IEnumerable<string>? Deserialize(string content, string fileName)
        {
            var i = content.IndexOf("guid:");
            if (i < 0)
            {
                return null;
            }
            var j = content.IndexOf("\n", i);
            if (j < 0)
            {
                return [content[(i + 5)..].Trim()];
            }
            return [content[(i + 5)..j].Trim()];
        }

        public override string Serialize(IEnumerable<string> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
