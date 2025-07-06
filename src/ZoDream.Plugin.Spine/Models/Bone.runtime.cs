using System;

namespace ZoDream.Plugin.Spine.Models
{
    internal class BoneRuntime(Bone bone)
    {
        public float Rotate { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ShearX { get; set; }
        public float ShearY { get; set; }


        public float A { get; set; }
        public float B { get; set; }
        public float C { get; set; }
        public float D { get; set; }

        public float WorldX { get; set; }
        public float WorldY { get; set; }
        public float WorldRotationX => Atan2Deg(C, A);
        public float WorldRotationY => Atan2Deg(D, B);

        public float WorldScaleX => (float)Math.Sqrt(A * A + C * C);
        public float WorldScaleY => (float)Math.Sqrt(B * B + D * D);
        public static float Atan2Deg(float x, float y)
        {
            return (float)Math.Atan2(y, x) * 180f / (float)Math.PI;
        }

        public void UpdateWorldTransform(SkeletonRoot skeleton)
        {
            UpdateWorldTransform(skeleton, bone.X, bone.Y, bone.Rotate, bone.ScaleX, bone.ScaleY, bone.ShearX, bone.ShearY);
        }

        public void UpdateWorldTransform(SkeletonRoot skeleton, float x, float y, float rotation, float scaleX, float scaleY, float shearX, float shearY)
        {
            X = x;
            Y = y;
            Rotate = rotation;
            ScaleX = scaleX;
            ScaleY = scaleY;
            ShearX = shearX;
            ShearY = shearY;

            var degRad = (float)Math.PI / 180;

            var parent = Array.Find(skeleton.Bones, i => i.Name == bone.Parent);
            if (parent == null)
            { // Root bone.
                float sx = skeleton.Runtime.ScaleX, sy = skeleton.Runtime.ScaleY;
                float rx = (rotation + shearX) * degRad;
                float ry = (rotation + 90 + shearY) * degRad;
                A = (float)Math.Cos(rx) * scaleX * sx;
                B = (float)Math.Cos(ry) * scaleY * sx;
                C = (float)Math.Sin(rx) * scaleX * sy;
                D = (float)Math.Sin(ry) * scaleY * sy;
                WorldX = x * sx + skeleton.Runtime.X;
                WorldY = y * sy + skeleton.Runtime.Y;
                return;
            }

            float pa = parent.Runtime.A, 
                pb = parent.Runtime.B, 
                pc = parent.Runtime.C, 
                pd = parent.Runtime.D;
            WorldX = pa * x + pb * y + parent.Runtime.WorldX;
            WorldY = pc * x + pd * y + parent.Runtime.WorldY;

            switch (bone.Transform)
            {
                case TransformMode.Normal:
                    {
                        float rx = (rotation + shearX) * degRad;
                        float ry = (rotation + 90 + shearY) * degRad;
                        float la = (float)Math.Cos(rx) * scaleX;
                        float lb = (float)Math.Cos(ry) * scaleY;
                        float lc = (float)Math.Sin(rx) * scaleX;
                        float ld = (float)Math.Sin(ry) * scaleY;
                        A = pa * la + pb * lc;
                        B = pa * lb + pb * ld;
                        C = pc * la + pd * lc;
                        D = pc * lb + pd * ld;
                        return;
                    }
                case TransformMode.OnlyTranslation:
                    {
                        float rx = (rotation + shearX) * degRad;
                        float ry = (rotation + 90 + shearY) * degRad;
                        A = (float)Math.Cos(rx) * scaleX;
                        B = (float)Math.Cos(ry) * scaleY;
                        C = (float)Math.Sin(rx) * scaleX;
                        D = (float)Math.Sin(ry) * scaleY;
                        break;
                    }
                case TransformMode.NoRotationOrReflection:
                    {
                        float sx = 1 / skeleton.Runtime.ScaleX, sy = 1 / skeleton.Runtime.ScaleY;
                        pa *= sx;
                        pc *= sy;
                        float s = pa * pa + pc * pc, prx;
                        if (s > 0.0001f)
                        {
                            s = Math.Abs(pa * pd * sy - pb * sx * pc) / s;
                            pb = pc * s;
                            pd = pa * s;
                            prx = Atan2Deg(pc, pa);
                        }
                        else
                        {
                            pa = 0;
                            pc = 0;
                            prx = 90 - Atan2Deg(pd, pb);
                        }
                        float rx = (rotation + shearX - prx) * degRad;
                        float ry = (rotation + shearY - prx + 90) * degRad;
                        float la = (float)Math.Cos(rx) * scaleX;
                        float lb = (float)Math.Cos(ry) * scaleY;
                        float lc = (float)Math.Sin(rx) * scaleX;
                        float ld = (float)Math.Sin(ry) * scaleY;
                        A = pa * la - pb * lc;
                        B = pa * lb - pb * ld;
                        C = pc * la + pd * lc;
                        D = pc * lb + pd * ld;
                        break;
                    }
                case TransformMode.NoScale:
                case TransformMode.NoScaleOrReflection:
                    {
                        rotation *= degRad;
                        float cos = (float)Math.Cos(rotation), sin = (float)Math.Sin(rotation);
                        float za = (pa * cos + pb * sin) / skeleton.Runtime.ScaleX;
                        float zc = (pc * cos + pd * sin) / skeleton.Runtime.ScaleY;
                        float s = (float)Math.Sqrt(za * za + zc * zc);
                        if (s > 0.00001f) s = 1 / s;
                        za *= s;
                        zc *= s;
                        s = (float)Math.Sqrt(za * za + zc * zc);
                        if (bone.Transform == TransformMode.NoScale 
                            && (pa * pd - pb * pc < 0) != (skeleton.Runtime.ScaleX < 0 != skeleton.Runtime.ScaleY < 0))
                        {
                            s = -s;
                        }
                        rotation = (float)(Math.PI / 2 + Math.Atan2(zc, za));
                        float zb = (float)Math.Cos(rotation) * s;
                        float zd = (float)Math.Sin(rotation) * s;
                        shearX *= degRad;
                        shearY = (90 + shearY) * degRad;
                        float la = (float)Math.Cos(shearX) * scaleX;
                        float lb = (float)Math.Cos(shearY) * scaleY;
                        float lc = (float)Math.Sin(shearX) * scaleX;
                        float ld = (float)Math.Sin(shearY) * scaleY;
                        A = za * la + zb * lc;
                        B = za * lb + zb * ld;
                        C = zc * la + zd * lc;
                        D = zc * lb + zd * ld;
                        break;
                    }
            }
            A *= skeleton.Runtime.ScaleX;
            B *= skeleton.Runtime.ScaleX;
            C *= skeleton.Runtime.ScaleY;
            D *= skeleton.Runtime.ScaleY;
        }

        public void UpdateAppliedTransform(SkeletonRoot skeleton)
        {
            var parent = Array.Find(skeleton.Bones, i => i.Name == bone.Parent);
            if (parent == null)
            {
                X = WorldX - skeleton.Runtime.X;
                Y = WorldY - skeleton.Runtime.Y;
                float a = A, b = B, c = C, d = D;
                Rotate = Atan2Deg(c, a);
                ScaleX = (float)Math.Sqrt(a * a + c * c);
                ScaleY = (float)Math.Sqrt(b * b + d * d);
                ShearX = 0;
                ShearY = Atan2Deg(a * b + c * d, a * d - b * c);
                return;
            }

            float pa = parent.Runtime.A, 
                pb = parent.Runtime.B, 
                pc = parent.Runtime.C, 
                pd = parent.Runtime.D;
            float pid = 1 / (pa * pd - pb * pc);
            float ia = pd * pid, 
                ib = pb * pid, 
                ic = pc * pid, 
                id = pa * pid;
            float dx = WorldX - parent.Runtime.WorldX, 
                dy = WorldY - parent.Runtime.WorldY;
            X = (dx * ia - dy * ib);
            Y = (dy * id - dx * ic);

            float ra, rb, rc, rd;
            if (bone.Transform == TransformMode.OnlyTranslation)
            {
                ra = A;
                rb = B;
                rc = C;
                rd = D;
            }
            else
            {
                switch (bone.Transform)
                {
                    case TransformMode.NoRotationOrReflection:
                        {
                            float s = Math.Abs(pa * pd - pb * pc) / (pa * pa + pc * pc);
                            float skeletonScaleY = skeleton.Runtime.ScaleY;
                            pb = -pc * skeleton.Runtime.ScaleX * s / skeletonScaleY;
                            pd = pa * skeletonScaleY * s / skeleton.Runtime.ScaleX;
                            pid = 1 / (pa * pd - pb * pc);
                            ia = pd * pid;
                            ib = pb * pid;
                            break;
                        }
                    case TransformMode.NoScale:
                    case TransformMode.NoScaleOrReflection:
                        {
                            float r = bone.Rotate * (float)Math.PI / 180, 
                                cos = (float)Math.Cos(r), 
                                sin = (float)Math.Sin(r);
                            pa = (pa * cos + pb * sin) / skeleton.Runtime.ScaleX;
                            pc = (pc * cos + pd * sin) / skeleton.Runtime.ScaleY;
                            float s = (float)Math.Sqrt(pa * pa + pc * pc);
                            if (s > 0.00001f) s = 1 / s;
                            pa *= s;
                            pc *= s;
                            s = (float)Math.Sqrt(pa * pa + pc * pc);
                            if (bone.Transform == TransformMode.NoScale 
                                && pid < 0 != (skeleton.Runtime.ScaleX < 0 != skeleton.Runtime.ScaleY < 0))
                            {
                                s = -s;
                            }
                            r = (float)(Math.PI / 2 + Math.Atan2(pc, pa));
                            pb = (float)Math.Cos(r) * s;
                            pd = (float)Math.Sin(r) * s;
                            pid = 1 / (pa * pd - pb * pc);
                            ia = pd * pid;
                            ib = pb * pid;
                            ic = pc * pid;
                            id = pa * pid;
                            break;
                        }
                }
                ra = ia * A - ib * C;
                rb = ia * B - ib * D;
                rc = id * C - ic * A;
                rd = id * D - ic * B;
            }

            ShearX = 0;
            ScaleX = (float)Math.Sqrt(ra * ra + rc * rc);
            if (ScaleX > 0.0001f)
            {
                float det = ra * rd - rb * rc;
                ScaleY = det / ScaleX;
                ShearY = -Atan2Deg(ra * rb + rc * rd, det);
                Rotate = Atan2Deg(rc, ra);
            }
            else
            {
                ScaleX = 0;
                ScaleY = (float)Math.Sqrt(rb * rb + rd * rd);
                ShearY = 0;
                Rotate = 90 - Atan2Deg(rd, rb);
            }
        }
    }
}
