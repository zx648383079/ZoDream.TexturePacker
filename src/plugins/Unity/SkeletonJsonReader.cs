using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.IO;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Unity
{
    public class SkeletonJsonReader : BaseTextReader<ISkeleton>, ISkeletonReader
    {
        private readonly JsonSerializerOptions _option = new()
        {
            PropertyNamingPolicy = new LowCaseJsonNamingPolicy()
        };
        public override bool IsEnabled(string content)
        {
            return content.Contains("\"armature\"") && content.Contains("\"aabb\"");
        }
        public override IEnumerable<ISkeleton>? Deserialize(string content, string fileName)
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
                    Bones = group.Bone.Select(item => {
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
                                var t = new SpriteLayer()
                                {
                                    Name = skin.Name,
                                };
                                skin.Display[0].TryParse(t);
                                bone.SkinItems.Add(t);
                            }
                        }
                        return bone;
                    }).ToArray(),
                    Animations = group.Animation.Select(item => {
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


        public override string Serialize(IEnumerable<ISkeleton> data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
