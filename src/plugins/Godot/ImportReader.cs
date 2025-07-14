using System;
using System.Collections.Generic;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;

namespace ZoDream.Plugin.Godot
{
    public class ImportReader : BaseTextReader<string>, IFileMetaReader
    {
        public override bool IsEnabled(string content)
        {
            return content.Contains("uid=\"");
        }
        protected override string GetFullPath(string fileName)
        {
            if (fileName.EndsWith(".import"))
            {
                return fileName;
            }
            return fileName + ".import";
        }
        public override IEnumerable<string>? Deserialize(string content, string fileName)
        {
            var uid = ReaderHelper.MatchWithRange(content, "uid=\"", "\"");
            return string.IsNullOrEmpty(uid) ? null : [uid];
        }

        public override string Serialize(IEnumerable<string> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
