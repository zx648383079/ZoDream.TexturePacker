using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Plugin.Spine.Models;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Spine
{
    public class SpineSkeletonController : ISkeletonController
    {
        public SpineSkeletonController(SkeletonRoot root)
        {
            _root = root;
            _root.Runtime ??= new SkeletonRuntime(this, root);
            root.Runtime.Skin = root.Skins.Length > 0 ? root.Skins[0] : null;
            Initialize();
        }

        private readonly SkeletonRoot _root;
        private readonly List<IUpdatableRuntime> _updateItems = [];

        internal bool YDown => false;

        internal float Time { get; private set; }

        internal SkeletonRoot Root => _root;
        internal Skin Skin => _root.Runtime.Skin;
        public float Width => _root.Skeleton.Width;

        public float Height => _root.Skeleton.Height;

        public IEnumerable<ISkeletonBone> Bones => _root.Bones;

        public IEnumerable<ISkeletonSkin> Skins => _root.Skins;

        public IEnumerable<ISkeletonSlot> Slots => _root.Slots;

        public IEnumerable<ISkeletonAnimation> Animations => _root.Animations;

        public IEnumerable<IReadOnlyStyle> Items {
            get {
                var items = new List<IReadOnlyStyle>();
                float[] vertices;
                foreach (var item in _root.Slots)
                {
                    switch (item.Runtime.Attachment)
                    {
                        case RegionAttachment region:
                            vertices = new float[8];
                            region.ComputeVertices(item, vertices, 0, 2);
                            items.Add(new SpriteUvLayer()
                            {
                                Name = item.Name,
                                VertexItems = Extension.ToVector(region.UVs),
                                PointItems = Extension.ToPoint(vertices, vertices.Length, 0, 0)
                            });
                            break;
                        case MeshAttachment mesh:
                            vertices = new float[mesh.WorldVerticesLength];
                            mesh.ComputeVertices(_root, item, 0, vertices.Length, vertices, 0, 2);
                            items.Add(new SpriteUvLayer()
                            {
                                Name = item.Name,
                                VertexItems = Extension.ToVector(mesh.UVs),
                                PointItems = Extension.ToPoint(vertices, vertices.Length, 0, 0)
                            });
                            break;
                    }
                }
                return items;
            }
        }


        private void Initialize()
        {
            _root.Runtime = new SkeletonRuntime(this, _root);
            var boneItems = _root.Bones.ToDictionary(i => i.Name);
            var slotItems = _root.Slots.ToDictionary(i => i.Name);
            foreach (var item in _root.Bones)
            {
                item.Runtime ??= new BoneRuntime(this, item);
                if (boneItems.TryGetValue(item.Parent, out var res))
                {
                    res.Runtime ??= new BoneRuntime(this, res);
                    item.Runtime.Parent = res;
                    res.Runtime.Children = [.. res.Runtime.Children, item];
                }
            }
            foreach (var item in _root.Slots)
            {
                item.Runtime = new SlotRuntime(this, item)
                {
                    Bone = boneItems[item.Bone]
                };
            }
            foreach (var item in _root.IkConstraints)
            {
                item.Runtime = new IkConstraintRuntime(this, item)
                {
                    Target = boneItems[item.Target],
                    Bones = item.Bones.Select(i => boneItems[i]).ToArray()
                };
            }
            foreach (var item in _root.TransformConstraints)
            {
                item.Runtime = new TransformConstraintRuntime(this, item)
                {
                    Target = boneItems[item.Target],
                    Bones = item.Bones.Select(i => boneItems[i]).ToArray()
                };
            }
            foreach (var item in _root.PathConstraints)
            {
                item.Runtime = new PathConstraintRuntime(this, item)
                {
                    Target = slotItems[item.Target],
                    Bones = item.Bones.Select(i => boneItems[i]).ToArray()
                };
            }
            foreach (var item in _root.PhysicsConstraints)
            {
                item.Runtime = new PhysicsConstraintRuntime(this, item)
                {
                    Bone = boneItems[item.Bone],
                };
            }
            Reset();
        }

        private void Reset()
        {
            _updateItems.Clear();
            foreach (var item in _root.Slots)
            {
                _root.Runtime.Skin ??= _root.Skins[0];
                if (_root.Runtime.Skin.TryGet<AttachmentBase>(item.Index, 
                    item.Attachment, out var res))
                {
                    item.Runtime.Attachment = res;
                }
            }
            var constraintCount = _root.IkConstraints.Length
                + _root.TransformConstraints.Length
                + _root.PathConstraints.Length
                + _root.PhysicsConstraints.Length;
            for (int i = 0; i < constraintCount; i++)
            {
                foreach (var item in _root.IkConstraints)
                {
                    if (item.Order == i)
                    {
                        SortIkConstraint(item);
                        break;
                    }
                }
                foreach (var item in _root.TransformConstraints)
                {
                    if (item.Order == i)
                    {
                        SortTransformConstraint(item);
                        break;
                    }
                }
                foreach (var item in _root.PathConstraints)
                {
                    if (item.Order == i)
                    {
                        SortPathConstraint(item);
                        break;
                    }
                }
                foreach (var item in _root.PhysicsConstraints)
                {
                    if (item.Order == i)
                    {
                        SortPhysicsConstraint(item);
                        break;
                    }
                }
            }
        }

        public void SetPose(string name)
        {

        }

        public void SetSkin(string name)
        {
            _root.Runtime.Skin = Array.Find(_root.Skins, i => i.Name == name);
            Reset();
        }

        public void SetSlot(string name)
        {

        }

        public void UpdateWorldTransform(PhysicsMode physics)
        {
            foreach (var bone in _root.Bones)
            {
                bone.Runtime.X = bone.X;
                bone.Runtime.Y = bone.Y;
                bone.Runtime.Rotate = bone.Rotate;
                bone.Runtime.ScaleX = bone.ScaleX;
                bone.Runtime.ScaleY = bone.ScaleY;
                bone.Runtime.ShearX = bone.ShearX;
                bone.Runtime.ShearY = bone.ShearY;
            }

            foreach (var item in _updateItems)
            {
                item.Update(physics);
            }
        }

        public void Update(float delta)
        {
            Time += delta;
        }


        private void SortIkConstraint(IkConstraint constraint)
        {
            constraint.Runtime.IsEnabled = constraint.Runtime.Target.Runtime.IsEnabled
                && (!constraint.SkinRequired || (Skin != null && Skin.Ik.Contains(constraint.Name)));
            if (!constraint.Runtime.IsEnabled)
            {
                return;
            }

            var target = constraint.Runtime.Target;
            SortBone(target);

            var constrained = constraint.Runtime.Bones;
            var parent = constrained[0];
            SortBone(parent);

            if (constrained.Length == 1)
            {
                _updateItems.Add(constraint.Runtime);
                SortReset(parent.Runtime.Children);
            }
            else
            {
                var child = constrained.Last();
                SortBone(child);

                _updateItems.Add(constraint.Runtime);

                SortReset(parent.Runtime.Children);
                child.Runtime.IsSorted = true;
            }
        }

        private void SortTransformConstraint(TransformConstraint constraint)
        {
            constraint.Runtime.IsEnabled = constraint.Runtime.Target.Runtime.IsEnabled
                && (!constraint.SkinRequired || (Skin != null && Skin.Transform.Contains(constraint.Name)));
            if (!constraint.Runtime.IsEnabled)
            {
                return;
            }

            SortBone(constraint.Runtime.Target);

            var constrained = constraint.Runtime.Bones;
            int boneCount = constraint.Bones.Length;
            if (constraint.Local)
            {
                for (int i = 0; i < boneCount; i++)
                {
                    var child = constrained[i];
                    SortBone(child.Runtime.Parent);
                    SortBone(child);
                }
            }
            else
            {
                for (int i = 0; i < boneCount; i++)
                {
                    SortBone(constrained[i]);
                }
            }

            _updateItems.Add(constraint.Runtime);

            for (int i = 0; i < boneCount; i++)
            {
                SortReset(constrained[i].Runtime.Children);
            }
            for (int i = 0; i < boneCount; i++)
            {
                constrained[i].Runtime.IsSorted = true;
            }
        }

        private void SortPathConstraint(PathConstraint constraint)
        {
            constraint.Runtime.IsEnabled = constraint.Runtime.Target.Runtime.Bone.Runtime.IsEnabled
                && (!constraint.SkinRequired || (Skin != null && Skin.Path.Contains(constraint.Name)));
            if (!constraint.Runtime.IsEnabled)
            {
                return;
            }

            var slot = constraint.Runtime.Target;
            int slotIndex = slot.Index;
            var slotBone = slot.Runtime.Bone;
            if (Skin != null)
            {
                SortPathConstraintAttachment(Skin, slotIndex, slotBone);
            }
            if (_root.Skins.Length > 0 && _root.Skins[0] != Skin)
            {
                SortPathConstraintAttachment(_root.Skins[0], slotIndex, slotBone);
            }

            var attachment = slot.Runtime.Attachment;
            if (attachment is PathAttachment)
            {
                SortPathConstraintAttachment(attachment, slotBone);
            }

            var constrained = constraint.Runtime.Bones;
            int boneCount = constraint.Bones.Length;
            for (int i = 0; i < boneCount; i++)
            {
                SortBone(constrained[i]);
            }

            _updateItems.Add(constraint.Runtime);

            for (int i = 0; i < boneCount; i++)
            {
                SortReset(constrained[i].Runtime.Children);
            }
            for (int i = 0; i < boneCount; i++)
            {
                constrained[i].Runtime.IsSorted = true;
            }
        }

        private void SortPathConstraintAttachment(Skin skin, int slotIndex, Bone slotBone)
        {
            foreach (var entry in skin.Attachments)
            {
                if (entry.Key.SlotIndex == slotIndex)
                {
                    SortPathConstraintAttachment(entry.Value, slotBone);
                }
            }
        }

        private void SortPathConstraintAttachment(AttachmentBase attachment, Bone slotBone)
        {
            if (!(attachment is PathAttachment))
            {
                return;
            }
            int[] pathBones = ((PathAttachment)attachment).Bones;
            if (pathBones == null)
            {
                SortBone(slotBone);
            }
            else
            {
                var bones = _root.Bones;
                for (int i = 0, n = pathBones.Length; i < n;)
                {
                    int nn = pathBones[i++];
                    nn += i;
                    while (i < nn)
                    {
                        SortBone(bones[pathBones[i++]]);
                    }
                }
            }
        }

        private void SortPhysicsConstraint(PhysicsConstraint constraint)
        {
            var bone = constraint.Runtime.Bone;
            constraint.Runtime.IsEnabled = bone.Runtime.IsEnabled
                && (!constraint.SkinRequired || (Skin != null && Skin.Physics.Contains(constraint.Name)));
            if (!constraint.Runtime.IsEnabled)
            {
                return;
            }

            SortBone(bone);

            _updateItems.Add(constraint.Runtime);

            SortReset(bone.Runtime.Children);
            bone.Runtime.IsSorted = true;
        }

        private void SortBone(Bone bone)
        {
            if (bone.Runtime.IsSorted)
            {
                return;
            }
            var parent = bone.Runtime.Parent;
            if (parent != null)
            {
                SortBone(parent);
            }
            bone.Runtime.IsSorted = true;
            _updateItems.Add(bone.Runtime);
        }

        private static void SortReset(Bone[] bones)
        {
            foreach (var item in bones)
            {
                if (!item.Runtime.IsEnabled)
                {
                    continue;
                }
                if (item.Runtime.IsSorted)
                {
                    SortReset(item.Runtime.Children);
                }
                item.Runtime.IsSorted = false;
            }
        }
    }
}
