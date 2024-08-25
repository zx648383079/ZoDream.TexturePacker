using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ZoDream.TexturePacker.Models;
using ZoDream.TexturePacker.Plugins.Readers.Unity;

namespace ZoDream.TexturePacker.Plugins.Readers.Spine
{
    public class SkeletonJsonReader : BaseTextReader<SkeletonSection>
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowcaseJsonNamingPolicy()
        };

        public override bool Canable(string content)
        {
            return content.Contains("\"skeleton\"") && content.Contains("\"spine\"");
        }
        public override IEnumerable<SkeletonSection>? Deserialize(string content, string fileName)
        {
            var data = JsonSerializer.Deserialize<SP_SkeletonRoot>(content, _option);
            if (data == null)
            {
                return null;
            }
            var res = new SkeletonSection()
            {
                Name = data.Skeleton.Hash,
            };
            foreach (var bone in data.Bones)
            {
                var b = new SkeletonBone()
                {
                    Name = bone.Name,
                    X = bone.X,
                    Parent = bone.Parent ?? string.Empty,
                    Y = bone.Y,
                    Rotate = bone.Rotation ?? 0,
                    Length = bone.Length ?? 0,
                };
                if (data.Skins[0].Attachments.TryGetValue(bone.Name, out var skin))
                {
                    foreach (var item in skin)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Value.Type))
                        {
                            // TODO UV
                            continue;
                        }
                        b.SkinItems.Add(new SkeletonBoneTexture()
                        {
                            Name = item.Key,
                            X = item.Value.X,
                            Y = item.Value.Y,
                            Height = item.Value.Height,
                            Width = item.Value.Width,
                            Rotate = item.Value.Rotation ?? 0
                        });
                    }
                }
                res.BoneItems.Add(b);
            }
            foreach (var item in data.Animations)
            {
                var a = new SkeletonAnimationSection()
                {
                    Name = item.Key,
                };
                foreach (var it in item.Value.Bones)
                {
                    var section = new SkeletonBoneAnimation()
                    {
                        Name = it.Key,
                    };
                    a.BoneItems.Add(section);
                    foreach (var frames in it.Value)
                    {
                        var prefix = frames.Key == "scale" ? "scale." : string.Empty;
                        foreach (var frame in frames.Value)
                        {
                            if (frames.Key == "rotate")
                            {
                                section.FrameItems.Add(new SkeletonAnimationFrame()
                                {
                                    Duration = frame.Time,
                                    PropertyName = frames.Key,
                                    TargetValue = frame.Angle ?? 0,
                                });
                                continue;
                            }
                            if (frame.X is not null)
                            {
                                section.FrameItems.Add(new SkeletonAnimationFrame()
                                {
                                    Duration = frame.Time,
                                    PropertyName = prefix + "x",
                                    TargetValue = frame.X ?? 0,
                                });
                            }
                            if (frame.Y is not null)
                            {
                                section.FrameItems.Add(new SkeletonAnimationFrame()
                                {
                                    Duration = frame.Time,
                                    PropertyName = prefix + "y",
                                    TargetValue = frame.Y ?? 0,
                                });
                            }
                        }
                    }
                }
                res.AnimationItems.Add(a);
            }
            return [res];
        }

        public override string Serialize(IEnumerable<SkeletonSection> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
