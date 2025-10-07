using System.Collections.Generic;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{

    public interface ISkeletonController : ISkeleton
    {
        /// <summary>
        /// 获取所有图块的摆放位置
        /// </summary>
        public IEnumerable<IReadOnlyStyle> Items { get; }

        public void SetSkin(string name);
        public void SetSlot(string name);
        public void SetPose(string name);
        /// <summary>
        /// 调用之后通过 Items 获取
        /// </summary>
        /// <param name="delta"></param>
        public void Update(float delta);
        public void Connect(ISpriteSection sprite);
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
