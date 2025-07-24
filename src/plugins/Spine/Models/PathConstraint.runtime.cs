using System;

namespace ZoDream.Plugin.Spine.Models
{
    public class PathConstraintRuntime(SpineSkeletonController controller, PathConstraint constraint) : IUpdatableRuntime
    {
        const int NONE = -1, BEFORE = -2, AFTER = -3;
        const float Epsilon = 0.00001f;
        public bool IsEnabled { get; set; }

        public Slot Target { get; set; }

        public Bone[] Bones { get; set; }

        private float[] _spaces = [];
        private float[] _positions = [];
        private float[] _world = [];
        private float[] _curves = [];
        private float[] _lengths = [];
        private readonly float[] _segments = new float[10];

        public void Update(PhysicsMode physics)
        {
            var attachment = Target.Runtime.Attachment as PathAttachment;
            if (attachment == null)
            {
                return;
            }

            float mixRotate = constraint.RotateMix, mixX = constraint.TranslateXMix, 
                mixY = constraint.TranslateYMix;
            if (mixRotate == 0 && mixX == 0 && mixY == 0)
            {
                return;
            }

            bool tangents = constraint.RotateMode == RotateMode.Tangent, 
                scale = constraint.RotateMode == RotateMode.ChainScale;
            int boneCount = Bones.Length, 
                spacesCount = tangents ? boneCount : boneCount + 1;
            Bone[] bonesItems = Bones;
            _spaces = new float[spacesCount];
            _lengths = scale ? new float[boneCount] : [];
            float spacing = constraint.Spacing;
            switch (constraint.SpacingMode)
            {
                case SpacingMode.Percent:
                    if (scale)
                    {
                        for (int i = 0, n = spacesCount - 1; i < n; i++)
                        {
                            Bone bone = bonesItems[i];
                            float setupLength = bone.Length ?? 0;
                            float x = setupLength * bone.Runtime.A, 
                                y = setupLength * bone.Runtime.C;
                            _lengths[i] = (float)Math.Sqrt(x * x + y * y);
                        }
                    }
                    ArraysFill(_spaces, 1, spacesCount, spacing);
                    break;
                case SpacingMode.Proportional:
                    {
                        float sum = 0;
                        for (int i = 0, n = spacesCount - 1; i < n;)
                        {
                            Bone bone = bonesItems[i];
                            float setupLength = bone.Length ?? 0;
                            if (setupLength < Epsilon)
                            {
                                if (scale)
                                {
                                    _lengths[i] = 0;
                                }
                                _spaces[++i] = spacing;
                            }
                            else
                            {
                                float x = setupLength * bone.Runtime.A, 
                                    y = setupLength * bone.Runtime.C;
                                float length = (float)Math.Sqrt(x * x + y * y);
                                if (scale)
                                {
                                    _lengths[i] = length;
                                }
                                _spaces[++i] = length;
                                sum += length;
                            }
                        }
                        if (sum > 0)
                        {
                            sum = spacesCount / sum * spacing;
                            for (int i = 1; i < spacesCount; i++)
                            {
                                _spaces[i] *= sum;
                            }
                        }
                        break;
                    }
                default:
                    {
                        bool lengthSpacing = constraint.SpacingMode == SpacingMode.Length;
                        for (int i = 0, n = spacesCount - 1; i < n;)
                        {
                            Bone bone = bonesItems[i];
                            float setupLength = bone.Length ?? 0;
                            if (setupLength < Epsilon)
                            {
                                if (scale)
                                {
                                    _lengths[i] = 0;
                                }
                                _spaces[++i] = spacing;
                            }
                            else
                            {
                                float x = setupLength * bone.Runtime.A, 
                                    y = setupLength * bone.Runtime.C;
                                float length = (float)Math.Sqrt(x * x + y * y);
                                if (scale)
                                {
                                    _lengths[i] = length;
                                }
                                _spaces[++i] = (lengthSpacing ? setupLength + spacing : spacing) * length / setupLength;
                            }
                        }
                        break;
                    }
            }

            float[] positions = ComputeWorldPositions(attachment, spacesCount, tangents);
            float boneX = positions[0], boneY = positions[1], 
                offsetRotation = constraint.OffsetRotation;
            bool tip;
            if (offsetRotation == 0)
            {
                tip = constraint.RotateMode == RotateMode.Chain;
            }
            else
            {
                tip = false;
                var p = Target.Runtime.Bone;
                offsetRotation *= p.Runtime.A * p.Runtime.D - p.Runtime.B * p.Runtime.C > 0 
                    ? (float)(Math.PI / 180) : -(float)(Math.PI / 180);
            }
            for (int i = 0, p = 3; i < boneCount; i++, p += 3)
            {
                var bone = bonesItems[i];
                bone.Runtime.WorldX += (boneX - bone.Runtime.WorldX) * mixX;
                bone.Runtime.WorldY += (boneY - bone.Runtime.WorldY) * mixY;
                float x = positions[p], y = positions[p + 1], dx = x - boneX, dy = y - boneY;
                if (scale)
                {
                    float length = _lengths[i];
                    if (length >= Epsilon)
                    {
                        float s = ((float)Math.Sqrt(dx * dx + dy * dy) / length - 1) * mixRotate + 1;
                        bone.Runtime.A *= s;
                        bone.Runtime.C *= s;
                    }
                }
                boneX = x;
                boneY = y;
                if (mixRotate > 0)
                {
                    float a = bone.Runtime.A, 
                        b = bone.Runtime.B, 
                        c = bone.Runtime.C, 
                        d = bone.Runtime.D, r, cos, sin;
                    if (tangents)
                    {
                        r = positions[p - 1];
                    }
                    else if (_spaces[i + 1] < Epsilon)
                    {
                        r = positions[p + 2];
                    }
                    else
                    {
                        r = (float)Math.Atan2(dy, dx);
                    }
                    r -= (float)Math.Atan2(c, a);
                    if (tip)
                    {
                        cos = (float)Math.Cos(r);
                        sin = (float)Math.Sin(r);
                        float length = bone.Length ?? 0;
                        boneX += (length * (cos * a - sin * c) - dx) * mixRotate;
                        boneY += (length * (sin * a + cos * c) - dy) * mixRotate;
                    }
                    else
                    {
                        r += offsetRotation;
                    }
                    if (r > Math.PI)
                    {
                        r -= (float)Math.PI * 2;
                    }
                    else if (r < -Math.PI) //
                    {
                        r += (float)Math.PI * 2;
                    }
                    r *= mixRotate;
                    cos = (float)Math.Cos(r);
                    sin = (float)Math.Sin(r);
                    bone.Runtime.A = cos * a - sin * c;
                    bone.Runtime.B = cos * b - sin * d;
                    bone.Runtime.C = sin * a + cos * c;
                    bone.Runtime.D = sin * b + cos * d;
                }
                bone.Runtime.UpdateAppliedTransform();
            }
        }

        float[] ComputeWorldPositions(PathAttachment path, int spacesCount, bool tangents)
        {
            var target = Target;
            float position = constraint.Position;
            _positions = new float[spacesCount * 3 + 2];
            float[] spaces = _spaces, 
                output = _positions, world;
            bool closed = path.Closed;
            int verticesLength = path.WorldVerticesLength, curveCount = verticesLength / 6, prevCurve = NONE;

            float pathLength, multiplier;
            if (!path.ConstantSpeed)
            {
                float[] lengths = path.Lengths;
                curveCount -= closed ? 1 : 2;
                pathLength = lengths[curveCount];

                if (constraint.PositionMode == PositionMode.Percent)
                {
                    position *= pathLength;
                }

                multiplier = constraint.SpacingMode switch
                {
                    SpacingMode.Percent => pathLength,
                    SpacingMode.Proportional => pathLength / spacesCount,
                    _ => 1,
                };
                _world = new float[8];
                world = _world;
                for (int i = 0, o = 0, curve = 0; i < spacesCount; i++, o += 3)
                {
                    float space = spaces[i] * multiplier;
                    position += space;
                    float p = position;

                    if (closed)
                    {
                        p %= pathLength;
                        if (p < 0) p += pathLength;
                        curve = 0;
                    }
                    else if (p < 0)
                    {
                        if (prevCurve != BEFORE)
                        {
                            prevCurve = BEFORE;
                            path.ComputeVertices(controller.Root, target, 2, 4, world, 0, 2);
                        }
                        AddBeforePosition(p, world, 0, output, o);
                        continue;
                    }
                    else if (p > pathLength)
                    {
                        if (prevCurve != AFTER)
                        {
                            prevCurve = AFTER;
                            path.ComputeVertices(controller.Root, target, verticesLength - 6, 4, world, 0, 2);
                        }
                        AddAfterPosition(p - pathLength, world, 0, output, o);
                        continue;
                    }

                    // Determine curve containing position.
                    for (; ; curve++)
                    {
                        float length = lengths[curve];
                        if (p > length) continue;
                        if (curve == 0)
                            p /= length;
                        else
                        {
                            float prev = lengths[curve - 1];
                            p = (p - prev) / (length - prev);
                        }
                        break;
                    }
                    if (curve != prevCurve)
                    {
                        prevCurve = curve;
                        if (closed && curve == curveCount)
                        {
                            path.ComputeVertices(controller.Root, target, verticesLength - 4, 4, world, 0, 2);
                            path.ComputeVertices(controller.Root, target, 0, 4, world, 4, 2);
                        }
                        else
                        {
                            path.ComputeVertices(controller.Root, target, curve * 6 + 2, 8, world, 0, 2);
                        }
                    }
                    AddCurvePosition(p, world[0], world[1], world[2], world[3], world[4], world[5], world[6], world[7], output, o,
                        tangents || (i > 0 && space < Epsilon));
                }
                return output;
            }

            // World vertices.
            if (closed)
            {
                verticesLength += 2;
                _world = new float[verticesLength];
                world = _world;
                path.ComputeVertices(controller.Root, target, 2, verticesLength - 4, world, 0, 2);
                path.ComputeVertices(controller.Root, target, 0, 2, world, verticesLength - 4, 2);
                world[verticesLength - 2] = world[0];
                world[verticesLength - 1] = world[1];
            }
            else
            {
                curveCount--;
                verticesLength -= 4;
                _world = new float[verticesLength];
                world = _world;
                path.ComputeVertices(controller.Root, target, 2, verticesLength, world, 0, 2);
            }

            // Curve lengths.
            _curves = new float[curveCount];
            float[] curves = _curves;
            pathLength = 0;
            float x1 = world[0], y1 = world[1], cx1 = 0, cy1 = 0, cx2 = 0, cy2 = 0, x2 = 0, y2 = 0;
            float tmpx, tmpy, dddfx, dddfy, ddfx, ddfy, dfx, dfy;
            for (int i = 0, w = 2; i < curveCount; i++, w += 6)
            {
                cx1 = world[w];
                cy1 = world[w + 1];
                cx2 = world[w + 2];
                cy2 = world[w + 3];
                x2 = world[w + 4];
                y2 = world[w + 5];
                tmpx = (x1 - cx1 * 2 + cx2) * 0.1875f;
                tmpy = (y1 - cy1 * 2 + cy2) * 0.1875f;
                dddfx = ((cx1 - cx2) * 3 - x1 + x2) * 0.09375f;
                dddfy = ((cy1 - cy2) * 3 - y1 + y2) * 0.09375f;
                ddfx = tmpx * 2 + dddfx;
                ddfy = tmpy * 2 + dddfy;
                dfx = (cx1 - x1) * 0.75f + tmpx + dddfx * 0.16666667f;
                dfy = (cy1 - y1) * 0.75f + tmpy + dddfy * 0.16666667f;
                pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                dfx += ddfx;
                dfy += ddfy;
                ddfx += dddfx;
                ddfy += dddfy;
                pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                dfx += ddfx;
                dfy += ddfy;
                pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                dfx += ddfx + dddfx;
                dfy += ddfy + dddfy;
                pathLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                curves[i] = pathLength;
                x1 = x2;
                y1 = y2;
            }

            if (constraint.PositionMode == PositionMode.Percent)
            {
                position *= pathLength;
            }

            multiplier = constraint.SpacingMode switch
            {
                SpacingMode.Percent => pathLength,
                SpacingMode.Proportional => pathLength / spacesCount,
                _ => 1,
            };
            float[] segments = _segments;
            float curveLength = 0;
            for (int i = 0, o = 0, curve = 0, segment = 0; i < spacesCount; i++, o += 3)
            {
                float space = spaces[i] * multiplier;
                position += space;
                float p = position;

                if (closed)
                {
                    p %= pathLength;
                    if (p < 0) p += pathLength;
                    curve = 0;
                }
                else if (p < 0)
                {
                    AddBeforePosition(p, world, 0, output, o);
                    continue;
                }
                else if (p > pathLength)
                {
                    AddAfterPosition(p - pathLength, world, verticesLength - 4, output, o);
                    continue;
                }

                // Determine curve containing position.
                for (; ; curve++)
                {
                    float length = curves[curve];
                    if (p > length) continue;
                    if (curve == 0)
                        p /= length;
                    else
                    {
                        float prev = curves[curve - 1];
                        p = (p - prev) / (length - prev);
                    }
                    break;
                }

                // Curve segment lengths.
                if (curve != prevCurve)
                {
                    prevCurve = curve;
                    int ii = curve * 6;
                    x1 = world[ii];
                    y1 = world[ii + 1];
                    cx1 = world[ii + 2];
                    cy1 = world[ii + 3];
                    cx2 = world[ii + 4];
                    cy2 = world[ii + 5];
                    x2 = world[ii + 6];
                    y2 = world[ii + 7];
                    tmpx = (x1 - cx1 * 2 + cx2) * 0.03f;
                    tmpy = (y1 - cy1 * 2 + cy2) * 0.03f;
                    dddfx = ((cx1 - cx2) * 3 - x1 + x2) * 0.006f;
                    dddfy = ((cy1 - cy2) * 3 - y1 + y2) * 0.006f;
                    ddfx = tmpx * 2 + dddfx;
                    ddfy = tmpy * 2 + dddfy;
                    dfx = (cx1 - x1) * 0.3f + tmpx + dddfx * 0.16666667f;
                    dfy = (cy1 - y1) * 0.3f + tmpy + dddfy * 0.16666667f;
                    curveLength = (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                    segments[0] = curveLength;
                    for (ii = 1; ii < 8; ii++)
                    {
                        dfx += ddfx;
                        dfy += ddfy;
                        ddfx += dddfx;
                        ddfy += dddfy;
                        curveLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                        segments[ii] = curveLength;
                    }
                    dfx += ddfx;
                    dfy += ddfy;
                    curveLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                    segments[8] = curveLength;
                    dfx += ddfx + dddfx;
                    dfy += ddfy + dddfy;
                    curveLength += (float)Math.Sqrt(dfx * dfx + dfy * dfy);
                    segments[9] = curveLength;
                    segment = 0;
                }

                // Weight by segment length.
                p *= curveLength;
                for (; ; segment++)
                {
                    float length = segments[segment];
                    if (p > length) continue;
                    if (segment == 0)
                        p /= length;
                    else
                    {
                        float prev = segments[segment - 1];
                        p = segment + (p - prev) / (length - prev);
                    }
                    break;
                }
                AddCurvePosition(p * 0.1f, x1, y1, cx1, cy1, cx2, cy2, x2, y2, output, o, tangents || (i > 0 && space < Epsilon));
            }
            return output;
        }

        static void AddBeforePosition(float p, float[] temp, int i, float[] output, int o)
        {
            float x1 = temp[i], y1 = temp[i + 1], dx = temp[i + 2] - x1, dy = temp[i + 3] - y1, r = (float)Math.Atan2(dy, dx);
            output[o] = x1 + p * (float)Math.Cos(r);
            output[o + 1] = y1 + p * (float)Math.Sin(r);
            output[o + 2] = r;
        }

        static void AddAfterPosition(float p, float[] temp, int i, float[] output, int o)
        {
            float x1 = temp[i + 2], y1 = temp[i + 3], dx = x1 - temp[i], dy = y1 - temp[i + 1], r = (float)Math.Atan2(dy, dx);
            output[o] = x1 + p * (float)Math.Cos(r);
            output[o + 1] = y1 + p * (float)Math.Sin(r);
            output[o + 2] = r;
        }

        static void AddCurvePosition(float p, float x1, float y1, float cx1, float cy1, float cx2, float cy2, float x2, float y2,
            float[] output, int o, bool tangents)
        {
            if (p < Epsilon || float.IsNaN(p))
            {
                output[o] = x1;
                output[o + 1] = y1;
                output[o + 2] = (float)Math.Atan2(cy1 - y1, cx1 - x1);
                return;
            }
            float tt = p * p, ttt = tt * p, u = 1 - p, uu = u * u, uuu = uu * u;
            float ut = u * p, ut3 = ut * 3, uut3 = u * ut3, utt3 = ut3 * p;
            float x = x1 * uuu + cx1 * uut3 + cx2 * utt3 + x2 * ttt, y = y1 * uuu + cy1 * uut3 + cy2 * utt3 + y2 * ttt;
            output[o] = x;
            output[o + 1] = y;
            if (tangents)
            {
                if (p < 0.001f)
                    output[o + 2] = (float)Math.Atan2(cy1 - y1, cx1 - x1);
                else
                    output[o + 2] = (float)Math.Atan2(y - (y1 * uu + cy1 * ut * 2 + cy2 * tt), x - (x1 * uu + cx1 * ut * 2 + cx2 * tt));
            }
        }

        public static void ArraysFill(float[] a, int fromIndex, int toIndex, float val)
        {
            for (int i = fromIndex; i < toIndex; i++)
            {
                a[i] = val;
            }
        }
    }
}
