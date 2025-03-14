using System;

namespace ZoDream.Plugin.Spine.Models
{
    internal class VertexAttachment: AttachmentBase
    {
        public int Id { get; set; }
        public int[] Bones { get; set; }
        public float[] Vertices { get; set; }
        public int WorldVerticesLength { get; set; }


        public void ComputeVertices(
            SkeletonRoot skeleton,
            Slot slot, float[] worldVertices)
        {
            ComputeVertices(
                skeleton,
                slot, 0, WorldVerticesLength, worldVertices, 0);
        }

        public virtual void ComputeVertices(
            SkeletonRoot skeleton,
            Slot slot, int start, int count, float[] worldVertices, int offset, int stride = 2)
        {
            count = offset + (count >> 1) * stride;
            var deformArray = slot.Runtime.Deform;
            var vertices = Vertices;
            var bones = Bones;
            if (bones == null)
            {
                if (deformArray.Count > 0)
                {
                    vertices = [.. deformArray];
                }
                var bone = Array.Find(skeleton.Bones, i => i.Name == slot.Bone);
                float x = bone.Runtime.WorldX, 
                    y = bone.Runtime.WorldY;
                float a = bone.Runtime.A, b = bone.Runtime.B, c = bone.Runtime.C, d = bone.Runtime.D;
                for (int vv = start, w = offset; w < count; vv += 2, w += stride)
                {
                    float vx = vertices[vv], vy = vertices[vv + 1];
                    worldVertices[w] = vx * a + vy * b + x;
                    worldVertices[w + 1] = vx * c + vy * d + y;
                }
                return;
            }
            int v = 0, skip = 0;
            for (int i = 0; i < start; i += 2)
            {
                int n = bones[v];
                v += n + 1;
                skip += n;
            }
            var skeletonBones = skeleton.Bones;
            if (deformArray.Count == 0)
            {
                for (int w = offset, b = skip * 3; w < count; w += stride)
                {
                    float wx = 0, wy = 0;
                    int n = bones[v++];
                    n += v;
                    for (; v < n; v++, b += 3)
                    {
                        var bone = skeletonBones[bones[v]];
                        float vx = vertices[b], vy = vertices[b + 1], weight = vertices[b + 2];
                        wx += (vx * bone.Runtime.A + vy * bone.Runtime.B + bone.Runtime.WorldX) * weight;
                        wy += (vx * bone.Runtime.C + vy * bone.Runtime.D + bone.Runtime.WorldY) * weight;
                    }
                    worldVertices[w] = wx;
                    worldVertices[w + 1] = wy;
                }
            }
            else
            {
                float[] deform = [.. deformArray];
                for (int w = offset, b = skip * 3, f = skip << 1; w < count; w += stride)
                {
                    float wx = 0, wy = 0;
                    int n = bones[v++];
                    n += v;
                    for (; v < n; v++, b += 3, f += 2)
                    {
                        Bone bone = skeletonBones[bones[v]];
                        float vx = vertices[b] + deform[f], vy = vertices[b + 1] + deform[f + 1], weight = vertices[b + 2];
                        wx += (vx * bone.Runtime.A + vy * bone.Runtime.B + bone.Runtime.WorldX) * weight;
                        wy += (vx * bone.Runtime.C + vy * bone.Runtime.D + bone.Runtime.WorldY) * weight;
                    }
                    worldVertices[w] = wx;
                    worldVertices[w + 1] = wy;
                }
            }
        }
    }
}
