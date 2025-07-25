using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Live2d
{
    public class CubismSkeletonController(CubismModel root) : ISkeletonController
    {
        public float Width => root.GetCanvasSize().X;

        public float Height => root.GetCanvasSize().Y;

        public IEnumerable<ISkeletonBone> Bones => throw new NotImplementedException();

        public IEnumerable<ISkeletonSkin> Skins => throw new NotImplementedException();

        public IEnumerable<ISkeletonSlot> Slots => throw new NotImplementedException();

        public IEnumerable<ISkeletonAnimation> Animations => throw new NotImplementedException();

        public IEnumerable<IReadOnlyStyle> Items => throw new NotImplementedException();

        public void SetPose(string name)
        {
        }

        public void SetSkin(string name)
        {
        }

        public void SetSlot(string name)
        {
        }

        public void Update(float delta)
        {
            root.Update();
        }
    }
}
