﻿using SkiaSharp;
using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class MeshAttachment : VertexAttachment, IHasTextureRegion
    {
        public int HullLength { get; set; }
        public float[] RegionUVs { get; set; }
        public int[] Triangles { get; set; }

        public SKColor? Color { get; set; }

        public string Path { get; set; }
        public object RendererObject { get; set; }
        public float RegionU { get; set; }
        public float RegionV { get; set; }
        public float RegionU2 { get; set; }
        public float RegionV2 { get; set; }
        public bool RegionRotate { get; set; }
        public float RegionOffsetX { get; set; }
        public float RegionOffsetY { get; set; }
        public float RegionWidth { get; set; }
        public float RegionHeight { get; set; }
        public float RegionOriginalWidth { get; set; }
        public float RegionOriginalHeight { get; set; }

        public bool InheritDeform { get; set; }

        public int[] Edges { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Sequence? Sequence { get; internal set; }

        [JsonIgnore]
        public float[] UVs { get; set; }

        [JsonIgnore]
        public TextureRegion Region { get; set; }


        public void UpdateRegion()
        {
            if (UVs?.Length != RegionUVs.Length)
            {
                UVs = new float[RegionUVs.Length];
            }
            var uvs = UVs;
            var textureWidth = Region.Width / (Region.Uv2.X - Region.Uv.X);
            var textureHeight = Region.Height / (Region.Uv2.Y - Region.Uv.Y);
            var u = 0f;
            var v = 0f;
            var width = 0f;
            var height = 0f;
            if (Region is AtlasRegion region)
            {
                switch (region.Rotate)
                {
                    case 90:
                        u -= (region.OriginalWidth - region.OffsetY - region.Width) / textureWidth;
                        v -= (region.OriginalWidth - region.OffsetX - region.Height) / textureHeight;
                        width = region.OriginalHeight / textureWidth;
                        height = region.OriginalWidth / textureHeight;
                        for (var i = 0; i < RegionUVs.Length; i += 2)
                        {
                            uvs[i] = u + RegionUVs[i + 1] * width;
                            uvs[i + 1] = v + (1 - RegionUVs[i]) * height;
                        }
                        return;
                    case 180:
                        u -= (region.OriginalWidth - region.OffsetX - region.Width) / textureWidth;
                        v -= region.OffsetY / textureHeight;
                        width = region.OriginalWidth / textureWidth;
                        height = region.OriginalHeight / textureHeight;
                        for (var i = 0; i < RegionUVs.Length; i += 2)
                        {
                            uvs[i] = u + (1 - RegionUVs[i]) * width;
                            uvs[i + 1] = v + (1 - RegionUVs[i + 1]) * height;
                        }
                        return;
                    case 270:
                        u -= region.OffsetY / textureWidth;
                        v -= region.OffsetX / textureHeight;
                        width = region.OriginalHeight / textureWidth;
                        height = region.OriginalWidth / textureHeight;
                        for (int i = 0; i < RegionUVs.Length; i += 2)
                        {
                            uvs[i] = u + (1 - RegionUVs[i + 1]) * width;
                            uvs[i + 1] = v + RegionUVs[i] * height;
                        }
                        return;
                }
                u -= region.OffsetX / textureWidth;
                v -= (region.OriginalHeight - region.OffsetY - region.Height) / textureHeight;
                width = region.OriginalWidth / textureWidth;
                height = region.Height / textureHeight;
                
            }
            else if (Region == null)
            {
                u = v = 0;
                width = height = 1;
            }
            else
            {
                u = Region.Uv.X;
                v = Region.Uv.Y;
                width = Region.Uv2.X - u;
                height = Region.Uv2.Y - v;
            }
            for (int i = 0; i < RegionUVs.Length; i += 2)
            {
                uvs[i] = u + RegionUVs[i] * width;
                uvs[i + 1] = v + RegionUVs[i + 1] * height;
            }
        }

        public override void ComputeVertices(SkeletonRoot skeleton, Slot slot, int start, int count, float[] worldVertices, int offset, int stride = 2)
        {
            Sequence?.Apply(slot, this);
            base.ComputeVertices(skeleton, slot, start, count, worldVertices, offset, stride);
        }
    }
}
