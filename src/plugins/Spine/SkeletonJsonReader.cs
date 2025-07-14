using System;
using System.Collections.Generic;
using System.Text.Json;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Spine
{
    public partial class SkeletonJsonReader : BaseTextReader<SkeletonSection>, ISkeletonReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowCaseJsonNamingPolicy()
        };

        public override bool IsEnabled(string content)
        {
            return content.Contains("\"skeleton\"") && content.Contains("\"spine\"");
        }

        public override string Serialize(IEnumerable<SkeletonSection> data, string fileName)
        {
            throw new NotImplementedException();
        }

        
    }
}
