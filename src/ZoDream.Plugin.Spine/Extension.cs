using System;
using System.Linq;
using ZoDream.Plugin.Spine.Models;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Spine
{
    internal static class Extension
    {

        internal static SpriteLayerSection ToLayer(this AtlasPage page)
        {
            var res = new SpriteLayerSection()
            {
                Name = page.Name,
                Width = page.Width,
                Height = page.Height,
                Items = page.Items.Select(item => {
                    var layer = new SpriteLayer()
                    {
                        Name = item.Name,
                        X = item.X,
                        Y = item.Y,
                        Rotate = item.Rotate,
                        Width = item.Width,
                        Height = item.Height,
                    };
                    if (IsRotate90(item.Rotate))
                    {
                        layer.Width = item.Height;
                        layer.Height = item.Width;
                    }
                    return layer;
                }).ToList()
            };
            return res;
        }

        private static bool IsRotate90(int deg)
        {
            return Math.Abs(deg) % 180 == 90;
        }

        internal static SkeletonSection ToSkeleton(this SkeletonRoot data)
        {
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
                foreach (var item in data.Skins[0].Attachments)
                {
                    if (item.Key.Name != bone.Name)
                    {
                        continue;
                    }
                    if (item.Value is not RegionAttachment region)
                    {
                        // TODO UV
                        continue;
                    }
                    b.SkinItems.Add(new SkeletonBoneTexture()
                    {
                        Name = item.Key.Name,
                        X = region.X,
                        Y = region.Y,
                        Height = region.Height,
                        Width = region.Width,
                        Rotate = region.Rotation
                    });
                }
                res.BoneItems.Add(b);
            }
            //foreach (var item in data.Animations)
            //{
            //}
            return res;
        }
    }
}
