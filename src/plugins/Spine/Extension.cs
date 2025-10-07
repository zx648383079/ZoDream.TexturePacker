using SkiaSharp;
using System;
using System.Linq;
using System.Numerics;
using ZoDream.Plugin.Spine.Models;
using ZoDream.Shared.Interfaces;
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
                        Rotate = - item.Rotate,
                        Width = item.Width,
                        Height = item.Height,
                    };
                    if (IsRotate90(item.Rotate))
                    {
                        layer.Width = item.Height;
                        layer.Height = item.Width;
                    }
                    return (ISpriteLayer)layer;
                }).ToList()
            };
            return res;
        }

        private static bool IsRotate90(float deg)
        {
            return Math.Abs(deg) % 180 == 90;
        }

        internal static SkeletonSection ToSkeleton(this SkeletonRoot data)
        {
            var res = new SkeletonSection()
            {
                Name = data.Skeleton.Hash,
                Width = data.Skeleton.Width,
                Height = data.Skeleton.Height,
            };
            if (data.Skins.Length == 0)
            {
                return res;
            }
            res.Slots = data.Slots.Select(i => new SkeletonSlot()
            {
                Name = i.Name,
            }).ToArray();
            res.Skins = data.Skins.Select(i => new SkeletonSkin()
            {
                Name = i.Name,
            }).ToArray();
            foreach (var bone in data.Bones)
            {
                bone.Runtime.UpdateWorldTransform();
                var b = new SkeletonBone()
                {
                    Name = bone.Name,
                    Parent = bone.Parent ?? string.Empty,
                    Y = bone.Y,
                    X = bone.X,
                    Rotate = bone.Rotate,
                    ScaleX = bone.ScaleX,
                    ScaleY = bone.ScaleY,
                    ShearX = bone.ShearX,
                    ShearY = bone.ShearY,
                    Length = bone.Length ?? 0,
                };
                foreach (var item in data.Skins[0].Attachments)
                {
                    var slot = data.Slots[item.Key.SlotIndex];
                    if (slot.Bone != bone.Name)
                    {
                        continue;
                    }
                    if (item.Value is RegionAttachment region)
                    {
                        b.SkinItems.Add(new SpriteLayer()
                        {
                            Name = item.Key.Name,
                            X = region.X,
                            Y = region.Y,
                            ScaleX = region.ScaleX,
                            ScaleY = region.ScaleY,
                            Height = region.Height,
                            Width = region.Width,
                            Rotate = region.Rotation
                        });
                    } else if (item.Value is MeshAttachment mesh)
                    {
                        var vertices = new float[mesh.WorldVerticesLength];
                        mesh.ComputeVertices(data, slot, vertices);
                        b.SkinItems.Add(new SpriteUvLayer()
                        {
                            Name = item.Key.Name,
                            X = mesh.RegionOffsetX,
                            Y = mesh.RegionOffsetY,
                            Height = mesh.Height,
                            Width = mesh.Width,
                            VertexItems = ToVector(mesh.RegionUVs),
                            PointItems = ToPoint(vertices,
                                mesh.RegionUVs.Length,
                                data.Skeleton.X, 
                                data.Skeleton.Y)
                        });
                    }
                }
                res.Bones.Add(b);
            }
            return res;
        }

        internal static Vector2[] ToVector(float[] items)
        {
            if (items is null)
            {
                return [];
            }
            var res = new Vector2[items.Length / 2];
            for (var i = 0; i < res.Length; i ++ )
            {
                var j = i * 2;
                res[i] = new Vector2(items[j], items[j + 1]);
            }
            return res;
        }

        internal static SKPoint[] ToPoint(float[] items, 
            int count,
            float x, float y)
        {
            var res = new SKPoint[count / 2];
            for (var i = 0; i < res.Length; i ++ )
            {
                var j = i * 2;
                res[i] = new SKPoint(items[j] - x, items[j + 1] - y);
            }
            return res;
        }
    }
}
