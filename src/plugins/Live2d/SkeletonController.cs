using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Live2d
{
    public class CubismSkeletonController(CubismModel root) : ISkeletonController
    {

        public readonly string[] ParameterIds = root.GetParameterIds();
        public readonly string[] PartIds = root.GetPartIds();
        public readonly string[] DrawableIds = root.GetDrawableIds();

        public float Width => root.GetCanvasSize().X;

        public float Height => root.GetCanvasSize().Y;

        public IEnumerable<ISkeletonBone> Bones => [];

        public IEnumerable<ISkeletonSkin> Skins => [];

        public IEnumerable<ISkeletonSlot> Slots => PartIds.Select(i => new SkeletonSlot() {Name = i});

        public IEnumerable<ISkeletonAnimation> Animations => [];

        public IEnumerable<IReadOnlyStyle> Items {
            get {
                var items = new List<IReadOnlyStyle>();
                var slotCount = DrawableIds.Length;
                for (int i = 0; i < slotCount; i++)
                {
                    var vertexCount = root.GetDrawableVertexCount(i);
                    items.Add(new SpriteUvLayer()
                    {
                        Name = DrawableIds[i],
                        VertexItems = root.GetDrawableVertexUvs(i, vertexCount),
                        PointItems = Array.ConvertAll(root.GetDrawableVertexPositions(i, vertexCount), i => (SKPoint)i)
                    });
                }
                return items;
            }
        }

        public void SetPose(string name)
        {
        }

        public void SetSkin(string name)
        {
        }

        public void SetSlot(string name)
        {
        }

        public void SetAnimation(string name)
        {

        }

        public void Update(float delta)
        {
            root.Update();
        }

        public void Connect(ISpriteSection sprite)
        {

        }

        public void Dispose()
        {
            root.Dispose();
        }
    }
}
