using System;

namespace ZoDream.Plugin.Spine.Models
{
    public class PhysicsConstraintRuntime(SpineSkeletonController controller, PhysicsConstraint constraint) : IUpdatableRuntime
    {

        private bool _reset = true;
        private float _ux;
        private float _uy;
        private float _cx;
        private float _cy;
        private float _tx;
        private float _ty;
        private float _xOffset;
        private float _xVelocity;
        private float _yOffset;
        private float _yVelocity;
        private float _rotateOffset;
        private float _rotateVelocity;
        private float _scaleOffset;
        private float _scaleVelocity;
        private float _remaining;
        private float _lastTime;
        public bool IsEnabled { get; set; }

        public Bone Bone { get; set; }



        public void Reset()
        {
            _remaining = 0;
            _lastTime = controller.Time;
            _reset = true;
            _xOffset = 0;
            _xVelocity = 0;
            _yOffset = 0;
            _yVelocity = 0;
            _rotateOffset = 0;
            _rotateVelocity = 0;
            _scaleOffset = 0;
            _scaleVelocity = 0;
        }

        public void Update(PhysicsMode physics)
        {
            float mix = constraint.Mix;
            if (mix == 0) return;

            bool x = constraint.X > 0, y = constraint.Y > 0, 
                rotateOrShearX = constraint.Rotate > 0 || constraint.ShearX > 0, 
                scaleX = constraint.ScaleX > 0;
            Bone bone = Bone;
            float l = bone.Length ?? 0;

            switch (physics)
            {
                case PhysicsMode.None:
                    return;
                case PhysicsMode.Reset:
                    Reset();
                    goto case PhysicsMode.Update; // Fall through.
                case PhysicsMode.Update:
                    var skeleton = controller.Root;
                    float delta = Math.Max(controller.Time - _lastTime, 0);
                    _remaining += delta;
                    _lastTime = controller.Time;

                    float bx = bone.Runtime.WorldX, by = bone.Runtime.WorldY;
                    if (_reset)
                    {
                        _reset = false;
                        _ux = bx;
                        _uy = by;
                    }
                    else
                    {
                        float a = _remaining, i = constraint.Inertia, t = constraint.Step, f = skeleton.Skeleton.ReferenceScale, d = -1;
                        float qx = constraint.Limit * delta, qy = qx * Math.Abs(skeleton.Runtime.ScaleY);
                        qx *= Math.Abs(skeleton.Runtime.ScaleX);

                        if (x || y)
                        {
                            if (x)
                            {
                                float u = (_ux - bx) * i;
                                _xOffset += u > qx ? qx : u < -qx ? -qx : u;
                                _ux = bx;
                            }
                            if (y)
                            {
                                float u = (_uy - by) * i;
                                _yOffset += u > qy ? qy : u < -qy ? -qy : u;
                                _uy = by;
                            }
                            if (a >= t)
                            {
                                d = (float)Math.Pow(constraint.Damping, 60 * t);
                                float m = constraint.MassInverse * t, 
                                    e = constraint.Strength, 
                                    w = constraint.Wind * f, 
                                    g = (controller.YDown ? -constraint.Gravity : constraint.Gravity) * f;
                                do
                                {
                                    if (x)
                                    {
                                        _xVelocity += (w - _xOffset * e) * m;
                                        _xOffset += _xVelocity * t;
                                        _xVelocity *= d;
                                    }
                                    if (y)
                                    {
                                        _yVelocity -= (g + _yOffset * e) * m;
                                        _yOffset += _yVelocity * t;
                                        _yVelocity *= d;
                                    }
                                    a -= t;
                                } while (a >= t);
                            }
                            if (x) bone.Runtime.WorldX += _xOffset * mix * constraint.X;
                            if (y) bone.Runtime.WorldY += _yOffset * mix * constraint.Y;
                        }
                        if (rotateOrShearX || scaleX)
                        {
                            float ca = (float)Math.Atan2(bone.Runtime.C, bone.Runtime.A), c, s, mr = 0;
                            float dx = _cx - bone.Runtime.WorldX, dy = _cy - bone.Runtime.WorldY;
                            if (dx > qx)
                                dx = qx;
                            else if (dx < -qx)
                                dx = -qx;
                            if (dy > qy)
                                dy = qy;
                            else if (dy < -qy)
                                dy = -qy;
                            if (rotateOrShearX)
                            {
                                mr = (constraint.Rotate + constraint.ShearX) * mix;
                                float r = (float)Math.Atan2(dy + _ty, dx + _tx) - ca - _rotateOffset * mr;
                                _rotateOffset += (float)(r - (float)Math.Ceiling(r * 1 / Math.PI / 2 - 0.5f) * Math.PI * 2) * i;
                                r = _rotateOffset * mr + ca;
                                c = (float)Math.Cos(r);
                                s = (float)Math.Sin(r);
                                if (scaleX)
                                {
                                    r = l * bone.Runtime.WorldScaleX;
                                    if (r > 0) _scaleOffset += (dx * c + dy * s) * i / r;
                                }
                            }
                            else
                            {
                                c = (float)Math.Cos(ca);
                                s = (float)Math.Sin(ca);
                                float r = l * bone.Runtime.WorldScaleX;
                                if (r > 0) _scaleOffset += (dx * c + dy * s) * i / r;
                            }
                            a = _remaining;
                            if (a >= t)
                            {
                                if (d == -1) d = (float)Math.Pow(constraint.Damping, 60 * t);
                                float m = constraint.MassInverse * t, e = constraint.Strength, w = constraint.Wind, g = (controller.YDown ? -constraint.Gravity : constraint.Gravity), h = l / f;
                                while (true)
                                {
                                    a -= t;
                                    if (scaleX)
                                    {
                                        _scaleVelocity += (w * c - g * s - _scaleOffset * e) * m;
                                        _scaleOffset += _scaleVelocity * t;
                                        _scaleVelocity *= d;
                                    }
                                    if (rotateOrShearX)
                                    {
                                        _rotateVelocity -= ((w * s + g * c) * h + _rotateOffset * e) * m;
                                        _rotateOffset += _rotateVelocity * t;
                                        _rotateVelocity *= d;
                                        if (a < t) break;
                                        float r = _rotateOffset * mr + ca;
                                        c = (float)Math.Cos(r);
                                        s = (float)Math.Sin(r);
                                    }
                                    else if (a < t) //
                                        break;
                                }
                            }
                        }
                        _remaining = a;
                    }
                    _cx = bone.Runtime.WorldX;
                    _cy = bone.Runtime.WorldY;
                    break;
                case PhysicsMode.Pose:
                    if (x) bone.Runtime.WorldX += _xOffset * mix * constraint.X;
                    if (y) bone.Runtime.WorldY += _yOffset * mix * constraint.Y;
                    break;
            }

            if (rotateOrShearX)
            {
                float o = _rotateOffset * mix, s, c, a;
                if (constraint.ShearX > 0)
                {
                    float r = 0;
                    if (constraint.Rotate > 0)
                    {
                        r = o * constraint.Rotate;
                        s = (float)Math.Sin(r);
                        c = (float)Math.Cos(r);
                        a = bone.Runtime.B;
                        bone.Runtime.B = c * a - s * bone.Runtime.D;
                        bone.Runtime.D = s * a + c * bone.Runtime.D;
                    }
                    r += o * constraint.ShearX;
                    s = (float)Math.Sin(r);
                    c = (float)Math.Cos(r);
                    a = bone.Runtime.A;
                    bone.Runtime.A = c * a - s * bone.Runtime.C;
                    bone.Runtime.C = s * a + c * bone.Runtime.C;
                }
                else
                {
                    o *= constraint.Rotate;
                    s = (float)Math.Sin(o);
                    c = (float)Math.Cos(o);
                    a = bone.Runtime.A;
                    bone.Runtime.A = c * a - s * bone.Runtime.C;
                    bone.Runtime.C = s * a + c * bone.Runtime.C;
                    a = bone.Runtime.B;
                    bone.Runtime.B = c * a - s * bone.Runtime.D;
                    bone.Runtime.D = s * a + c * bone.Runtime.D;
                }
            }
            if (scaleX)
            {
                float s = 1 + _scaleOffset * mix * constraint.ScaleX;
                bone.Runtime.A *= s;
                bone.Runtime.C *= s;
            }
            if (physics != PhysicsMode.Pose)
            {
                _tx = l * bone.Runtime.A;
                _ty = l * bone.Runtime.C;
            }
            bone.Runtime.UpdateAppliedTransform();
        }
    }
}
