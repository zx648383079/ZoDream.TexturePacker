using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.EditorInterface;
using ZoDream.Shared.Models;
using ZoDream.Shared.Stylers;

namespace ZoDream.Shared.ImageEditor
{
    public class SkeletonImageStyler : IImageStyler, IImageComputedStyler, IImageSize
    {
        public SkeletonImageStyler(SkeletonSection skeleton)
            : this (skeleton.Name, skeleton)
        {
            
        }

        public SkeletonImageStyler(string name, SkeletonSection skeleton)
        {
            _name = string.IsNullOrWhiteSpace(name) ? "SKEL_" + DateTime.Now.Ticks : name;
            _skeleton = skeleton;
            Width = _skeleton.Width;
            Height = _skeleton.Height;
        }

        private readonly string _name;
        private readonly SkeletonSection _skeleton;

        private readonly List<ImageComputedStyle> _cacheItems = [];
        private ImageComputedStyle? _lastStyle;

        public string Name => _name;

        public float Width { get; set; }
        public float Height { get; set; }

        public float ActualWidth => Width;

        public float ActualHeight => Height;

        public IImageStyle Compute(IImageLayer layer)
        {
            if (_lastStyle?.LayerId == layer.Id)
            {
                return _lastStyle;
            }
            foreach (var item in _cacheItems)
            {
                if (item.LayerId == layer.Id)
                {
                    return item;
                }
            }
            return new ImageComputedStyle();
        }

        public void Compute(IImageLayerTree items)
        {
            var boneStyler = new ComputedStyler();
            foreach (var bone in _skeleton.BoneItems)
            {
                var style = boneStyler.Compute(bone.Name, bone, bone.Name);
                foreach (var item in bone.SkinItems)
                {
                    var layer = items.Get(i => i.Name == item.Name);
                    if (layer is null)
                    {
                        continue;
                    }
                    if (item is SpriteUvLayer u)
                    {
                        var computed = new ImageComputedVertexStyle(layer.Id)
                        {
                            X = style.X + item.X,
                            Y = style.Y + item.Y,
                            Rotate = style.Rotate + item.Rotate,
                            ScaleX = style.ScaleX * item.ScaleX,
                            ScaleY = style.ScaleY * item.ScaleY,
                            Width = item.Width,
                            Height = item.Height,
                        };
                        var matrix = style.ToMatrix();
                        computed.SourceItems = EditorExtension.ComputeVertex(u.VertexItems, layer.Source);
                        computed.PointItems = matrix.MapPoints([.. u.PointItems]);
                        _cacheItems.Add(computed);
                        continue;
                    }
                    _cacheItems.Add(new ImageComputedStyle(layer.Id)
                    {
                        X = style.X + item.X,
                        Y = style.Y + item.Y,
                        Rotate = style.Rotate + item.Rotate,
                        ScaleX = style.ScaleX * item.ScaleX,
                        ScaleY = style.ScaleY * item.ScaleY,
                        ShearX = style.ShearX + item.ShearX,
                        ShearY = style.ShearY + item.ShearY,
                        Width = item.Width,
                        Height = item.Height,
                    });
                }
            }
        }

        public void Paint(IImageLayerTree items, IImageCanvas canvas)
        {
            foreach (var item in _cacheItems)
            {
                var layer = items.Get(item.LayerId);
                if (layer is null)
                {
                    continue;
                }
                if (!layer.IsVisible)
                {
                    continue;
                }
                layer.Source?.Paint(canvas, item);
            }
        }

        public void Paint(IImageLayerTree items, SKCanvas canvas)
        {
            Paint(items, new ImageCanvas(canvas, this));
        }

        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKPoint point)
        {
            foreach (var item in _cacheItems.OrderByDescending(i => i.ZIndex))
            {
                if (!item.ToRect().Contains(point))
                {
                    continue;
                }
                var layer = items.Get(item.LayerId);
                if (layer is null || !layer.IsVisible)
                {
                    continue;
                }
                _lastStyle = item;
                yield return layer;
            }
        }

        public IEnumerable<IImageLayer> Where(IImageLayerTree items, SKRect rect)
        {
            foreach (var item in _cacheItems.OrderByDescending(i => i.ZIndex))
            {
                if (!item.ToRect().IntersectsWith(rect))
                {
                    continue;
                }
                var layer = items.Get(item.LayerId);
                if (layer is null || !layer.IsVisible)
                {
                    continue;
                }
                _lastStyle = item;
                yield return layer;
            }
        }

        public void Clear()
        {
            _cacheItems.Clear();
        }
    }
}
