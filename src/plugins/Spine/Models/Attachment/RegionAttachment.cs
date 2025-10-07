using SkiaSharp;
using System;
using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Spine.Models
{
    public class RegionAttachment : AttachmentBase, IHasTextureRegion
    {
        public const int BLX = 0;
        public const int BLY = 1;
        public const int ULX = 2;
        public const int ULY = 3;
        public const int URX = 4;
        public const int URY = 5;
        public const int BRX = 6;
        public const int BRY = 7;

        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public SKColor? Color { get; set; }

        public string Path { get; set; }
        public object RendererObject { get; set; }
        public float RegionOffsetX { get; set; }
        public float RegionOffsetY { get; set; }
        public float RegionWidth { get; set; }
        public float RegionHeight { get; set; }
        public float RegionOriginalWidth { get; set; }
        public float RegionOriginalHeight { get; set; }

        public float[] Offset { get; set; } = new float[8];


        public Sequence? Sequence { get; internal set; }

        [JsonIgnore]
        public float[] UVs { get; set; } = new float[8];
        [JsonIgnore]
        public TextureRegion Region { get; set; }


        public void UpdateRegion()
        {
            float[] uvs = UVs;
            if (Region == null)
            {
                uvs[BLX] = 0;
                uvs[BLY] = 0;
                uvs[ULX] = 0;
                uvs[ULY] = 1;
                uvs[URX] = 1;
                uvs[URY] = 1;
                uvs[BRX] = 1;
                uvs[BRY] = 0;
                return;
            }

            float width = Width, height = Height;
            float localX2 = width / 2;
            float localY2 = height / 2;
            float localX = -localX2;
            float localY = -localY2;
            bool rotated = false;
            if (Region is AtlasRegion region)
            {
                localX += region.OffsetX / region.OriginalWidth * width;
                localY += region.OffsetY / region.OriginalHeight * height;
                if (region.Rotate == 90)
                {
                    rotated = true;
                    localX2 -= (region.OriginalWidth - region.OffsetX - region.Height) / region.OriginalWidth * width;
                    localY2 -= (region.OriginalHeight - region.OffsetY - region.Width) / region.OriginalHeight * height;
                }
                else
                {
                    localX2 -= (region.OriginalWidth - region.OffsetX - region.Width) / region.OriginalWidth * width;
                    localY2 -= (region.OriginalHeight - region.OffsetY - region.Height) / region.OriginalHeight * height;
                }
            }
            float scaleX = ScaleX, scaleY = ScaleY;
            localX *= scaleX;
            localY *= scaleY;
            localX2 *= scaleX;
            localY2 *= scaleY;
            float r = Rotation * (float)Math.PI / 180, cos = (float)Math.Cos(r), sin = (float)Math.Sin(r);
            float x = X, y = Y;
            float localXCos = localX * cos + x;
            float localXSin = localX * sin;
            float localYCos = localY * cos + y;
            float localYSin = localY * sin;
            float localX2Cos = localX2 * cos + x;
            float localX2Sin = localX2 * sin;
            float localY2Cos = localY2 * cos + y;
            float localY2Sin = localY2 * sin;
            float[] offset = Offset;
            offset[BLX] = localXCos - localYSin;
            offset[BLY] = localYCos + localXSin;
            offset[ULX] = localXCos - localY2Sin;
            offset[ULY] = localY2Cos + localXSin;
            offset[URX] = localX2Cos - localY2Sin;
            offset[URY] = localY2Cos + localX2Sin;
            offset[BRX] = localX2Cos - localYSin;
            offset[BRY] = localYCos + localX2Sin;

            if (rotated)
            {
                uvs[BLX] = Region.Uv2.X;
                uvs[BLY] = Region.Uv.Y;
                uvs[ULX] = Region.Uv2.X;
                uvs[ULY] = Region.Uv2.Y;
                uvs[URX] = Region.Uv.X;
                uvs[URY] = Region.Uv2.Y;
                uvs[BRX] = Region.Uv.X;
                uvs[BRY] = Region.Uv.Y;
            }
            else
            {
                uvs[BLX] = Region.Uv2.X;
                uvs[BLY] = Region.Uv2.Y;
                uvs[ULX] = Region.Uv.X;
                uvs[ULY] = Region.Uv2.Y;
                uvs[URX] = Region.Uv.X;
                uvs[URY] = Region.Uv.Y;
                uvs[BRX] = Region.Uv2.X;
                uvs[BRY] = Region.Uv.Y;
            }
        }

        public void ComputeVertices(Slot slot, 
            float[] worldVertices, 
            int offset, 
            int stride = 2)
        {
            Sequence?.Apply(slot, this);

            float[] vertexOffset = Offset;
            var bone = slot.Runtime.Bone;
            float bwx = bone.Runtime.WorldX, bwy = bone.Runtime.WorldY;
            float a = bone.Runtime.A, b = bone.Runtime.B, c = bone.Runtime.C, d = bone.Runtime.D;
            float offsetX, offsetY;

            // Vertex order is different from RegionAttachment.java
            offsetX = vertexOffset[BRX]; // 0
            offsetY = vertexOffset[BRY]; // 1
            worldVertices[offset] = offsetX * a + offsetY * b + bwx; // bl
            worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
            offset += stride;

            offsetX = vertexOffset[BLX]; // 2
            offsetY = vertexOffset[BLY]; // 3
            worldVertices[offset] = offsetX * a + offsetY * b + bwx; // ul
            worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
            offset += stride;

            offsetX = vertexOffset[ULX]; // 4
            offsetY = vertexOffset[ULY]; // 5
            worldVertices[offset] = offsetX * a + offsetY * b + bwx; // ur
            worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
            offset += stride;

            offsetX = vertexOffset[URX]; // 6
            offsetY = vertexOffset[URY]; // 7
            worldVertices[offset] = offsetX * a + offsetY * b + bwx; // br
            worldVertices[offset + 1] = offsetX * c + offsetY * d + bwy;
            //offset += stride;
        }
    }
}
