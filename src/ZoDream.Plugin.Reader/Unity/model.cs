using System;
using System.Collections.Generic;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers.Unity
{
    public class U3D_SkeletonRoot
    {
        public int FrameRate { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string CompatibleVersion { get; set; }

        public U3D_Armature[] Armature { get; set; }
    }

    public class U3D_Armature
    {
        public string Type { get; set; }

        public int FrameRate { get; set; }

        public string Name { get; set; }

        public U3D_Aabb Aabb { get; set; }

        public U3D_ArmatureBone[] Bone { get; set; }

        public U3D_ArmatureSlot[] Slot { get; set; }

        public U3D_Ik[] Ik { get; set; }

        public U3D_Skin[] Skin { get; set; }

        public U3D_Animation[] Animation { get; set; }

        public U3D_DefaultAction[] DefaultActions { get; set; }
    }

    public class U3D_Aabb
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }

    public class U3D_Animation
    {
        public int Duration { get; set; }

        public int PlayTimes { get; set; }

        public string Name { get; set; }

        public U3D_AnimationBone[] Bone { get; set; }

        public U3D_AnimationFrame[] Frame { get; set; }

        public U3D_ZOrder ZOrder { get; set; }
    }

    public class U3D_AnimationBone
    {
        public string Name { get; set; }

        public U3D_TranslateFrame[] TranslateFrame { get; set; }

        public U3D_TranslateFrame[] RotateFrame { get; set; }

        public U3D_TranslateFrame[] ScaleFrame { get; set; }

        public IList<SkeletonAnimationFrame> Parse()
        {
            var items = new List<SkeletonAnimationFrame>();
            TryParse(items, TranslateFrame);
            TryParse(items, RotateFrame);
            TryParse(items, ScaleFrame, "scale.");
            return items;
        }

        private void TryParse(List<SkeletonAnimationFrame> items, 
            U3D_TranslateFrame[]? data, string prefix = "")
        {
            if (data is not null)
            {
                foreach (var item in data)
                {
                    if (item.X is not null)
                    {
                        items.Add(new SkeletonAnimationFrame()
                        {
                            Duration = item.Duration,
                            Easing = item.TweenEasing is null ? SkeletonEasing.Curve : (SkeletonEasing)item.TweenEasing,
                            CurveItems = item.Curve ?? [],
                            PropertyName = prefix + "x",
                            TargetValue = (float)item.X
                        });
                    }
                    if (item.Y is not null)
                    {
                        items.Add(new SkeletonAnimationFrame()
                        {
                            Duration = item.Duration,
                            Easing = item.TweenEasing is null ? SkeletonEasing.Curve : (SkeletonEasing)item.TweenEasing,
                            CurveItems = item.Curve ?? [],
                            PropertyName = prefix + "y",
                            TargetValue = (float)item.Y
                        });
                    }
                    if (item.Rotate is not null)
                    {
                        items.Add(new SkeletonAnimationFrame()
                        {
                            Duration = item.Duration,
                            Easing = item.TweenEasing is null ? SkeletonEasing.Curve : (SkeletonEasing)item.TweenEasing,
                            CurveItems = item.Curve ?? [],
                            PropertyName = "rotate",
                            TargetValue = (float)item.Rotate
                        });
                    }
                }
            }
        }
    }


    public class U3D_TranslateFrame
    {
        public int Duration { get; set; }

        public int? TweenEasing { get; set; }

        public float? X { get; set; }

        public float[] Curve { get; set; }

        public float? Y { get; set; }

        public float? Rotate { get; set; }
    }

    public class U3D_AnimationFrame
    {
        public long Duration { get; set; }

        public U3D_Event[] Events { get; set; }
    }

    public class U3D_Event
    {
        public string Name { get; set; }
    }

    public partial class U3D_ZOrder
    {
        public U3D_ZOrderFrame[] Frame { get; set; }
    }

    public partial class U3D_ZOrderFrame
    {
        public long Duration { get; set; }

        public long[] ZOrder { get; set; }
    }

    public class U3D_ArmatureBone
    {
        public string Name { get; set; }

        public Dictionary<string, float> Transform { get; set; }

        public string Parent { get; set; }

        public float? Length { get; set; }

        public void TryParse(SkeletonBone bone)
        {
            bone.X = Transform.TryGetValue("x", out var x) ? x : 0;
            bone.Y = Transform.TryGetValue("y", out x) ? x : 0;
            if (!Transform.TryGetValue("skX", out var skX) || 
                !Transform.TryGetValue("skY", out var skY))
            {
                return;
            }
            if (skX == bone.X && skY == bone.Y)
            {
                return;
            }
            if (skY == bone.Y)
            {
                bone.Rotate = skX > bone.Y ? 90 : 270;
                return;
            }
            bone.Rotate = (float)(Math.Atan((double)((skX - bone.X) / (skY - bone.Y))) / Math.PI * 180);
        }
    }

    public class U3D_DefaultAction
    {
        public string GotoAndPlay { get; set; }
    }

    public class U3D_Ik
    {
        public bool? BendPositive { get; set; }

        public long Chain { get; set; }

        public string Name { get; set; }

        public string Bone { get; set; }

        public string Target { get; set; }
    }

    public partial class U3D_Skin
    {
        public U3D_SkinSlot[] Slot { get; set; }
    }

    public class U3D_SkinSlot
    {
        public string Name { get; set; }

        public U3D_Display[] Display { get; set; }
    }

    public class U3D_Display
    {
        public string Name { get; set; }

        public Dictionary<string, float> Transform { get; set; }

        public void TryParse(SkeletonBoneTexture bone)
        {
            bone.X = Transform.TryGetValue("x", out var x) ? x : 0;
            bone.Y = Transform.TryGetValue("y", out x) ? x : 0;
            if (!Transform.TryGetValue("skX", out var skX) ||
                !Transform.TryGetValue("skY", out var skY))
            {
                return;
            }
            if (skX == bone.X && skY == bone.Y)
            {
                return;
            }
            // TODO 求宽高
            if (skY == bone.Y)
            {
                bone.Rotate = skX > bone.Y ? 90 : 270;
                return;
            }
            bone.Rotate = (float)(Math.Atan((double)((skX - bone.X) / (skY - bone.Y))) / Math.PI * 180);
        }
    }

    public class U3D_ArmatureSlot
    {
        public string Name { get; set; }

        public string Parent { get; set; }

        public long? DisplayIndex { get; set; }
    }
}
