using System;

namespace ZoDream.Plugin.Spine.Models
{
    public class IkConstraintRuntime(SpineSkeletonController controller, IkConstraint constraint) : IUpdatableRuntime
    {
        public bool IsEnabled { get; set; }

        public Bone Target { get; set; }

        public Bone[] Bones { get; set; }

        public void Update(PhysicsMode physics)
        {
            if (constraint.Mix == 0)
            {
                return;
            }
            var target = Target;
            var bones = Bones;
            switch (bones.Length)
            {
                case 1:
                    Apply(bones[0], target.Runtime.WorldX, target.Runtime.WorldY, 
                        constraint.Compress, constraint.Stretch, constraint.Uniform, constraint.Mix);
                    break;
                case 2:
                    Apply(bones[0], bones[1], target.Runtime.WorldX, target.Runtime.WorldY, constraint.BendDirection, 
                        constraint.Stretch, constraint.Uniform, constraint.Softness, constraint.Mix);
                    break;
            }
        }

        public void Apply(Bone bone, float targetX, float targetY, bool compress, bool stretch, bool uniform,
                                float alpha)
        {
            ArgumentNullException.ThrowIfNull(bone);
            var skeleton = controller.Root.Runtime;
            var p = bone.Runtime.Parent;

            float pa = p.Runtime.A, pb = p.Runtime.B, pc = p.Runtime.C, pd = p.Runtime.D;
            float rotationIK = -bone.Runtime.ShearX - bone.Runtime.Rotate;
            float tx = 0, ty = 0;

            switch (bone.Transform)
            {
                case TransformMode.OnlyTranslation:
                    tx = (targetX - bone.Runtime.WorldX) * Math.Sign(skeleton.ScaleX);
                    ty = (targetY - bone.Runtime.WorldY) * Math.Sign(skeleton.ScaleY);
                    break;
                case TransformMode.NoRotationOrReflection:
                    {
                        float s = Math.Abs(pa * pd - pb * pc) / Math.Max(0.0001f, pa * pa + pc * pc);
                        float sa = pa / skeleton.ScaleX;
                        float sc = pc / skeleton.ScaleY;
                        pb = -sc * s * skeleton.ScaleX;
                        pd = sa * s * skeleton.ScaleY;
                        rotationIK += BoneRuntime.Atan2Deg(sc, sa);
                        goto default; // Fall through.
                    }
                default:
                    {
                        float x = targetX - p.Runtime.WorldX, y = targetY - p.Runtime.WorldY;
                        float d = pa * pd - pb * pc;
                        if (Math.Abs(d) <= 0.0001f)
                        {
                            tx = 0;
                            ty = 0;
                        }
                        else
                        {
                            tx = (x * pd - y * pb) / d - bone.Runtime.X;
                            ty = (y * pa - x * pc) / d - bone.Runtime.Y;
                        }
                        break;
                    }
            }

            rotationIK += BoneRuntime.Atan2Deg(ty, tx);
            if (bone.Runtime.ScaleX < 0) rotationIK += 180;
            if (rotationIK > 180)
                rotationIK -= 360;
            else if (rotationIK < -180) //
                rotationIK += 360;

            float sx = bone.Runtime.ScaleX, sy = bone.Runtime.ScaleY;
            if (compress || stretch)
            {
                switch (bone.Transform)
                {
                    case TransformMode.NoScale:
                    case TransformMode.NoScaleOrReflection:
                        tx = targetX - bone.Runtime.WorldX;
                        ty = targetY - bone.Runtime.WorldY;
                        break;
                }
                float b = (bone.Length ?? 0) * sx;
                if (b > 0.0001f)
                {
                    float dd = tx * tx + ty * ty;
                    if ((compress && dd < b * b) || (stretch && dd > b * b))
                    {
                        float s = ((float)Math.Sqrt(dd) / b - 1) * alpha + 1;
                        sx *= s;
                        if (uniform) sy *= s;
                    }
                }
            }
            bone.Runtime.UpdateWorldTransform(bone.Runtime.X, bone.Runtime.Y, 
                bone.Runtime.Rotate + rotationIK * alpha, sx, sy, 
                bone.Runtime.ShearX, bone.Runtime.ShearY);
        }

        public void Apply(Bone parent, Bone child, float targetX, float targetY, int bendDir, bool stretch, bool uniform,
            float softness, float alpha)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(child);
            if (parent.Transform != TransformMode.Normal || child.Transform != TransformMode.Normal)
            {
                return;
            }
            float px = parent.Runtime.X, py = parent.Runtime.Y, 
                psx = parent.Runtime.ScaleX, 
                psy = parent.Runtime.ScaleY, sx = psx, sy = psy, 
                csx = child.Runtime.ScaleX;
            int os1, os2, s2;
            if (psx < 0)
            {
                psx = -psx;
                os1 = 180;
                s2 = -1;
            }
            else
            {
                os1 = 0;
                s2 = 1;
            }
            if (psy < 0)
            {
                psy = -psy;
                s2 = -s2;
            }
            if (csx < 0)
            {
                csx = -csx;
                os2 = 180;
            }
            else
                os2 = 0;
            float cx = child.Runtime.X, cy, cwx, cwy, 
                a = parent.Runtime.A, b = parent.Runtime.B, 
                c = parent.Runtime.C, 
                d = parent.Runtime.D;
            bool u = Math.Abs(psx - psy) <= 0.0001f;
            if (!u || stretch)
            {
                cy = 0;
                cwx = a * cx + parent.Runtime.WorldX;
                cwy = c * cx + parent.Runtime.WorldY;
            }
            else
            {
                cy = child.Runtime.Y;
                cwx = a * cx + b * cy + parent.Runtime.WorldX;
                cwy = c * cx + d * cy + parent.Runtime.WorldY;
            }
            Bone pp = parent.Runtime.Parent;
            a = pp.Runtime.A;
            b = pp.Runtime.B;
            c = pp.Runtime.C;
            d = pp.Runtime.D;
            float id = a * d - b * c, x = cwx - pp.Runtime.WorldX, 
                y = cwy - pp.Runtime.WorldY;
            id = Math.Abs(id) <= 0.0001f ? 0 : 1 / id;
            float dx = (x * d - y * b) * id - px, dy = (y * a - x * c) * id - py;
            float l1 = (float)Math.Sqrt(dx * dx + dy * dy), 
                l2 = (child.Length ?? 0) * csx, a1, a2;
            if (l1 < 0.0001f)
            {
                Apply(parent, targetX, targetY, false, stretch, false, alpha);
                child.Runtime.UpdateWorldTransform(cx, cy, 0, 
                    child.Runtime.ScaleX, 
                    child.Runtime.ScaleY, 
                    child.Runtime.ShearX, 
                    child.Runtime.ShearY);
                return;
            }
            x = targetX - pp.Runtime.WorldX;
            y = targetY - pp.Runtime.WorldY;
            float tx = (x * d - y * b) * id - px, ty = (y * a - x * c) * id - py;
            float dd = tx * tx + ty * ty;
            if (softness != 0)
            {
                softness *= psx * (csx + 1) * 0.5f;
                float td = (float)Math.Sqrt(dd), sd = td - l1 - l2 * psx + softness;
                if (sd > 0)
                {
                    float p = Math.Min(1, sd / (softness * 2)) - 1;
                    p = (sd - softness * (1 - p * p)) / td;
                    tx -= p * tx;
                    ty -= p * ty;
                    dd = tx * tx + ty * ty;
                }
            }
            if (u)
            {
                l2 *= psx;
                float cos = (dd - l1 * l1 - l2 * l2) / (2 * l1 * l2);
                if (cos < -1)
                {
                    cos = -1;
                    a2 = (float)Math.PI * bendDir;
                }
                else if (cos > 1)
                {
                    cos = 1;
                    a2 = 0;
                    if (stretch)
                    {
                        a = ((float)Math.Sqrt(dd) / (l1 + l2) - 1) * alpha + 1;
                        sx *= a;
                        if (uniform) sy *= a;
                    }
                }
                else
                    a2 = (float)Math.Acos(cos) * bendDir;
                a = l1 + l2 * cos;
                b = l2 * (float)Math.Sin(a2);
                a1 = (float)Math.Atan2(ty * a - tx * b, tx * a + ty * b);
            }
            else
            {
                a = psx * l2;
                b = psy * l2;
                float aa = a * a, bb = b * b, ta = (float)Math.Atan2(ty, tx);
                c = bb * l1 * l1 + aa * dd - aa * bb;
                float c1 = -2 * bb * l1, c2 = bb - aa;
                d = c1 * c1 - 4 * c2 * c;
                if (d >= 0)
                {
                    float q = (float)Math.Sqrt(d);
                    if (c1 < 0) q = -q;
                    q = -(c1 + q) * 0.5f;
                    float r0 = q / c2, r1 = c / q;
                    float r = Math.Abs(r0) < Math.Abs(r1) ? r0 : r1;
                    r0 = dd - r * r;
                    if (r0 >= 0)
                    {
                        y = (float)Math.Sqrt(r0) * bendDir;
                        a1 = ta - (float)Math.Atan2(y, r);
                        a2 = (float)Math.Atan2(y / psy, (r - l1) / psx);
                        goto break_outer; // break outer;
                    }
                }
                float minAngle = (float)Math.PI, minX = l1 - a, minDist = minX * minX, minY = 0;
                float maxAngle = 0, maxX = l1 + a, maxDist = maxX * maxX, maxY = 0;
                c = -a * l1 / (aa - bb);
                if (c >= -1 && c <= 1)
                {
                    c = (float)Math.Acos(c);
                    x = a * (float)Math.Cos(c) + l1;
                    y = b * (float)Math.Sin(c);
                    d = x * x + y * y;
                    if (d < minDist)
                    {
                        minAngle = c;
                        minDist = d;
                        minX = x;
                        minY = y;
                    }
                    if (d > maxDist)
                    {
                        maxAngle = c;
                        maxDist = d;
                        maxX = x;
                        maxY = y;
                    }
                }
                if (dd <= (minDist + maxDist) * 0.5f)
                {
                    a1 = ta - (float)Math.Atan2(minY * bendDir, minX);
                    a2 = minAngle * bendDir;
                }
                else
                {
                    a1 = ta - (float)Math.Atan2(maxY * bendDir, maxX);
                    a2 = maxAngle * bendDir;
                }
            }
        break_outer:
            float os = (float)Math.Atan2(cy, cx) * s2;
            float rotation = parent.Runtime.Rotate;
            a1 = (a1 - os) * 180f / (float)Math.PI + os1 - rotation;
            if (a1 > 180)
            {
                a1 -= 360;
            }
            else if (a1 < -180)
            {
                a1 += 360;
            }
            parent.Runtime.UpdateWorldTransform(px, py, rotation + a1 * alpha, sx, sy, 0, 0);
            rotation = child.Runtime.Rotate;
            a2 = ((a2 + os) * 180f / (float)Math.PI - child.Runtime.ShearX) * s2 + os2 - rotation;
            if (a2 > 180)
            {
                a2 -= 360;
            }
            else if (a2 < -180)
            {
                a2 += 360;
            }
            child.Runtime.UpdateWorldTransform(cx, cy, rotation + a2 * alpha, 
                child.Runtime.ScaleX, child.Runtime.ScaleY, 
                child.Runtime.ShearX, child.Runtime.ShearY);
        }
    }
}
