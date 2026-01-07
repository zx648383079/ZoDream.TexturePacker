using System;
using System.Collections.Generic;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Stylers
{
    public class SkeletonController(ISkeleton root) : ISkeletonController
    {
        public IEnumerable<IReadOnlyStyle> Items => [];

        public float Width => root.Width;

        public float Height => root.Height;

        public IEnumerable<ISkeletonBone> Bones => root.Bones;

        public IEnumerable<ISkeletonSkin> Skins => root.Skins;

        public IEnumerable<ISkeletonSlot> Slots => root.Slots;

        public IEnumerable<ISkeletonAnimation> Animations => root.Animations;

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
        }

        public void Connect(ISpriteSection sprite)
        {

        }

        public void Dispose()
        {
        }
    }
}
