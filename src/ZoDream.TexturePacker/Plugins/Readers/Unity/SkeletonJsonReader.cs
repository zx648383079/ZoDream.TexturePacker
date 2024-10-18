using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZoDream.TexturePacker.Models;

namespace ZoDream.TexturePacker.Plugins.Readers.Unity
{
    public class SkeletonJsonReader : BaseTextReader<SkeletonSection>
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowcaseJsonNamingPolicy()
        };
        public override bool IsEnabled(string content)
        {
            return content.Contains("\"armature\"") && content.Contains("\"aabb\"");
        }
        public override IEnumerable<SkeletonSection>? Deserialize(string content, string fileName)
        {
            var data = JsonSerializer.Deserialize<U3D_SkeletonRoot>(content, _option);
            if (data == null) 
            {
                return null;
            }
            return data.Armature.Select(group => {

                return new SkeletonSection()
                {
                    Name = group.Name,
                    FrameRate = group.FrameRate,
                    BoneItems = group.Bone.Select(item => {
                        var bone = new SkeletonBone()
                        {
                            Name = item.Name,
                            Parent = item.Parent ?? string.Empty,
                            Length = item.Length ?? 0,
                        };
                        item.TryParse(bone);
                        foreach (var skin in group.Skin[0].Slot)
                        {
                            if (skin.Name == item.Name)
                            {
                                var t = new SkeletonBoneTexture()
                                {
                                    Name = skin.Name,
                                };
                                skin.Display[0].TryParse(t);
                                bone.SkinItems.Add(t);
                            }
                        }
                        return bone;
                    }).ToArray(),
                    AnimationItems = group.Animation.Select(item => {
                        return new SkeletonAnimationSection()
                        {
                            PayTimes = item.PlayTimes,
                            Duration = item.Duration,
                            Name = item.Name,
                            BoneItems = item.Bone.Select(bone => {
                                return new SkeletonBoneAnimation()
                                {
                                    Name = bone.Name,
                                    FrameItems = bone.Parse()
                                };
                            }).ToArray(),
                        };
                    }).ToArray(),
                };
            });
        }


        public override string Serialize(IEnumerable<SkeletonSection> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
