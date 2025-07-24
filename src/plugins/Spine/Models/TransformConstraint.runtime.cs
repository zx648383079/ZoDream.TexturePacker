using System;

namespace ZoDream.Plugin.Spine.Models
{
    public class TransformConstraintRuntime(SpineSkeletonController controller, TransformConstraint constraint) : IUpdatableRuntime
    {
        public bool IsEnabled { get; set; }

        public Bone Target { get; set; }
        public Bone[] Bones { get; set; }

        public void Update(PhysicsMode physics)
        {
            if (constraint.RotateMix == 0 && constraint.TranslateXMix == 0 
                && constraint.TranslateYMix == 0 
                && constraint.ScaleXMix == 0 
                && constraint.ScaleYMix == 0 && constraint.ShearYMix == 0)
            {
                return;
            }
            if (constraint.Local)
            {
                if (constraint.Relative)
                {
                    ApplyRelativeLocal();
                }
                else
                {
                    ApplyAbsoluteLocal();
                }
            }
            else
            {
                if (constraint.Relative)
                {
                    ApplyRelativeWorld();
                }
                else
                {
                    ApplyAbsoluteWorld();
                }
            }
        }

        private void ApplyAbsoluteWorld()
        {
            float mixRotate = constraint.RotateMix, mixX = constraint.TranslateXMix, 
                mixY = constraint.TranslateYMix, mixScaleX = constraint.ScaleXMix,
            mixScaleY = constraint.ScaleYMix, mixShearY = constraint.ShearYMix;
            bool translate = mixX != 0 || mixY != 0;

            var target = Target;
            float ta = target.Runtime.A, tb = target.Runtime.B, tc = target.Runtime.C, 
                td = target.Runtime.D;
            float degRadReflect = ta * td - tb * tc > 0 ? (float)Math.PI / 180 : -(float)(Math.PI / 180);
            float offsetRotation = constraint.OffsetRotation * degRadReflect, offsetShearY = constraint.OffsetShearY * degRadReflect;

            Bone[] bones = Bones;
            for (int i = 0, n = Bones.Length; i < n; i++)
            {
                Bone bone = bones[i];

                if (mixRotate != 0)
                {
                    float a = bone.Runtime.A, b = bone.Runtime.B, c = bone.Runtime.C, d = bone.Runtime.D;
                    float r = (float)Math.Atan2(tc, ta) - (float)Math.Atan2(c, a) + offsetRotation;
                    if (r > Math.PI)
                        r -= (float)Math.PI * 2;
                    else if (r < -Math.PI) //
                        r += (float)Math.PI * 2;
                    r *= mixRotate;
                    float cos = (float)Math.Cos(r), sin = (float)Math.Sin(r);
                    bone.Runtime.A = cos * a - sin * c;
                    bone.Runtime.B = cos * b - sin * d;
                    bone.Runtime.C = sin * a + cos * c;
                    bone.Runtime.D = sin * b + cos * d;
                }

                if (translate)
                {
                    float tx, ty; //Vector2 temp = this.temp;
                    target.Runtime.LocalToWorld(constraint.OffsetX, constraint.OffsetY, out tx, out ty); //target.localToWorld(temp.set(constraint.OffsetX, constraint.OffsetY));
                    bone.Runtime.WorldX += (tx - bone.Runtime.WorldX) * mixX;
                    bone.Runtime.WorldY += (ty - bone.Runtime.WorldY) * mixY;
                }

                if (mixScaleX != 0)
                {
                    float s = (float)Math.Sqrt(bone.Runtime.A * bone.Runtime.A + bone.Runtime.C * bone.Runtime.C);
                    if (s != 0) s = (s + ((float)Math.Sqrt(ta * ta + tc * tc) - s + constraint.OffsetScaleX) * mixScaleX) / s;
                    bone.Runtime.A *= s;
                    bone.Runtime.C *= s;
                }
                if (mixScaleY != 0)
                {
                    float s = (float)Math.Sqrt(bone.Runtime.B * bone.Runtime.B + bone.Runtime.D * bone.Runtime.D);
                    if (s != 0) s = (s + ((float)Math.Sqrt(tb * tb + td * td) - s + constraint.OffsetScaleY) * mixScaleY) / s;
                    bone.Runtime.B *= s;
                    bone.Runtime.D *= s;
                }

                if (mixShearY > 0)
                {
                    float b = bone.Runtime.B, d = bone.Runtime.D;
                    float by = (float)Math.Atan2(d, b);
                    float r = (float)Math.Atan2(td, tb) - (float)Math.Atan2(tc, ta) - (by - (float)Math.Atan2(bone.Runtime.C, bone.Runtime.A));
                    if (r > Math.PI)
                        r -= (float)Math.PI * 2;
                    else if (r < -Math.PI) //
                        r += (float)Math.PI * 2;
                    r = by + (r + offsetShearY) * mixShearY;
                    float s = (float)Math.Sqrt(b * b + d * d);
                    bone.Runtime.B = (float)Math.Cos(r) * s;
                    bone.Runtime.D = (float)Math.Sin(r) * s;
                }

                bone.Runtime.UpdateAppliedTransform();
            }
        }

        private void ApplyRelativeWorld()
        {
            float mixRotate = constraint.RotateMix, mixX = constraint.TranslateXMix, mixY = constraint.TranslateYMix, mixScaleX = constraint.ScaleXMix,
            mixScaleY = constraint.ScaleYMix, mixShearY = constraint.ShearYMix;
            bool translate = mixX != 0 || mixY != 0;

            Bone target = Target;
            float ta = target.Runtime.A, tb = target.Runtime.B, tc = target.Runtime.C, td = target.Runtime.D;
            float degRadReflect = ta * td - tb * tc > 0 ? (float)Math.PI / 180 : -(float)(Math.PI / 180);
            float offsetRotation = constraint.OffsetRotation * degRadReflect, offsetShearY = constraint.OffsetShearY * degRadReflect;

            foreach (var bone in Bones)
            {
                if (mixRotate != 0)
                {
                    float a = bone.Runtime.A, b = bone.Runtime.B, 
                        c = bone.Runtime.C, d = bone.Runtime.D;
                    float r = (float)Math.Atan2(tc, ta) + offsetRotation;
                    if (r > Math.PI)
                        r -= (float)Math.PI * 2;
                    else if (r < -Math.PI) //
                        r += (float)Math.PI * 2;
                    r *= mixRotate;
                    float cos = (float)Math.Cos(r), sin = (float)Math.Sin(r);
                    bone.Runtime.A = cos * a - sin * c;
                    bone.Runtime.B = cos * b - sin * d;
                    bone.Runtime.C = sin * a + cos * c;
                    bone.Runtime.D = sin * b + cos * d;
                }

                if (translate)
                {
                    float tx, ty; //Vector2 temp = this.temp;
                    target.Runtime.LocalToWorld(constraint.OffsetX, constraint.OffsetY, out tx, out ty); //target.localToWorld(temp.set(constraint.OffsetX, constraint.OffsetY));
                    bone.Runtime.WorldX += tx * mixX;
                    bone.Runtime.WorldY += ty * mixY;
                }

                if (mixScaleX != 0)
                {
                    float s = ((float)Math.Sqrt(ta * ta + tc * tc) - 1 + constraint.OffsetScaleX) * mixScaleX + 1;
                    bone.Runtime.A *= s;
                    bone.Runtime.C *= s;
                }
                if (mixScaleY != 0)
                {
                    float s = ((float)Math.Sqrt(tb * tb + td * td) - 1 + constraint.OffsetScaleY) * mixScaleY + 1;
                    bone.Runtime.B *= s;
                    bone.Runtime.D *= s;
                }

                if (mixShearY > 0)
                {
                    float r = (float)Math.Atan2(td, tb) - (float)Math.Atan2(tc, ta);
                    if (r > Math.PI)
                        r -= (float)Math.PI * 2;
                    else if (r < -(float)Math.PI) //
                        r += (float)Math.PI * 2;
                    float b = bone.Runtime.B, d = bone.Runtime.D;
                    r = (float)Math.Atan2(d, b) + (r - (float)Math.PI / 2 + offsetShearY) * mixShearY;
                    float s = (float)Math.Sqrt(b * b + d * d);
                    bone.Runtime.B = (float)Math.Cos(r) * s;
                    bone.Runtime.D = (float)Math.Sin(r) * s;
                }

                bone.Runtime.UpdateAppliedTransform();
            }
        }

        private void ApplyAbsoluteLocal()
        {
            float mixRotate = constraint.RotateMix, mixX = constraint.TranslateXMix, mixY = constraint.TranslateYMix, mixScaleX = constraint.ScaleXMix,
            mixScaleY = constraint.ScaleYMix, mixShearY = constraint.ShearYMix;

            Bone target = Target;

            foreach (var bone in Bones)
            { 
                float rotation = bone.Runtime.Rotate;
                if (mixRotate != 0) rotation += (target.Runtime.Rotate - rotation + constraint.OffsetRotation) * mixRotate;

                float x = bone.Runtime.X, y = bone.Runtime.Y;
                x += (target.Runtime.X - x + constraint.OffsetX) * mixX;
                y += (target.Runtime.Y - y + constraint.OffsetY) * mixY;

                float scaleX = bone.Runtime.ScaleX, scaleY = bone.Runtime.ScaleY;
                if (mixScaleX != 0 && scaleX != 0)
                    scaleX = (scaleX + (target.Runtime.ScaleX - scaleX + constraint.OffsetScaleX) * mixScaleX) / scaleX;
                if (mixScaleY != 0 && scaleY != 0)
                    scaleY = (scaleY + (target.Runtime.ScaleY - scaleY + constraint.OffsetScaleY) * mixScaleY) / scaleY;

                float shearY = bone.Runtime.ShearY;
                if (mixShearY != 0) shearY += (target.Runtime.ShearY - shearY + constraint.OffsetShearY) * mixShearY;

                bone.Runtime.UpdateWorldTransform(x, y, rotation, scaleX, scaleY, bone.Runtime.ShearX, shearY);
            }
        }

        private void ApplyRelativeLocal()
        {
            float mixRotate = constraint.RotateMix, mixX = constraint.TranslateXMix, mixY = constraint.TranslateYMix, mixScaleX = constraint.ScaleXMix,
            mixScaleY = constraint.ScaleYMix, mixShearY = constraint.ShearYMix;

            Bone target = Target;

            foreach (var bone in Bones)
            {

                float rotation = bone.Runtime.Rotate + (target.Runtime.Rotate + constraint.OffsetRotation) * mixRotate;
                float x = bone.Runtime.X + (target.Runtime.X + constraint.OffsetX) * mixX;
                float y = bone.Runtime.Y + (target.Runtime.Y + constraint.OffsetY) * mixY;
                float scaleX = bone.Runtime.ScaleX * (((target.Runtime.ScaleX - 1 + constraint.OffsetScaleX) * mixScaleX) + 1);
                float scaleY = bone.Runtime.ScaleY * (((target.Runtime.ScaleY - 1 + constraint.OffsetScaleY) * mixScaleY) + 1);
                float shearY = bone.Runtime.ShearY + (target.Runtime.ShearY + constraint.OffsetShearY) * mixShearY;

                bone.Runtime.UpdateWorldTransform(x, y, rotation, scaleX, scaleY, bone.Runtime.ShearX, shearY);
            }
        }
    }
}
