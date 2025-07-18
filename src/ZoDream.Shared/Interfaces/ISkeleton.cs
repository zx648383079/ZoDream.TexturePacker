using System.Collections.Generic;

namespace ZoDream.Shared.Interfaces
{

    public interface ISkeletonController : ISkeleton
    {

        public void SetSkin(string name);
        public void SetSlot(string name);
        public void SetPose(string name);

        public void Update(float delta);


    }

    public interface ISkeleton
    {
        public float Width { get; }

        public float Height { get; }

        public IEnumerable<ISkeletonBone> Bones { get; }
        public IEnumerable<ISkeletonSkin> Skins { get; }
        public IEnumerable<ISkeletonSlot> Slots { get; }
        public IEnumerable<ISkeletonAnimation> Animations { get; }
    }

    public interface ISkeletonBone
    {
        public string Name { get; }

        public string Parent { get; }
    }

    public interface ISkeletonSkin
    {
        public string Name { get; }
    }

    public interface ISkeletonSlot
    {
        public string Name { get; }
    }

    public interface ISkeletonAnimation
    {
        public string Name { get; }
    }
}
