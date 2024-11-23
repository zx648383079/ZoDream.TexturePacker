using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ZoDream.Plugin.Readers.Spine
{
    public class SP_SkeletonRoot
    {
        public Dictionary<string, SP_Animation> Animations { get; set; }

        public SP_Bone[] Bones { get; set; }

        public SP_Skeleton Skeleton { get; set; }

        public SP_Skin[] Skins { get; set; }

        public SP_Slot[] Slots { get; set; }

        public SP_IkConstraint[] IkConstraints { get; set; }
    }

    public class SP_IkConstraint
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public string[] Bones { get; set; }

        public string Target { get; set; }

        public float Mix { get; set; }

        public byte BendDirection { get; set; }
    }
    public class SP_TransformConstraint
    {
        public string Name { get; set; }

        public int Order { get; set; }
        public string[] Bones { get; set; }
        public string Target { get; set; }
        public float RotateMix { get; set; }
        public float TranslateMix { get; set; }
        public float ScaleMix { get; set; }
        public float ShearMix { get; set; }

        public float OffsetRotation { get; set; }
        public float OffsetX { get; set; }
        public float OffsetY { get; set; }
        public float OffsetScaleX { get; set; }
        public float OffsetScaleY { get; set; }
        public float OffsetShearY { get; set; }

        public bool Relative { get; set; }
        public bool Local { get; set; }
    }

    public class SP_PathConstraint
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string[] Bones { get; set; }
        public string Target { get; set; }
        public SP_PositionMode PositionMode { get; set; }
        public SP_SpacingMode SpacingMode { get; set; }
        public SP_RotateMode RotateMode { get; set; }
        public float OffsetRotation { get; set; }
        public float Position { get; set; }
        public float Spacing { get; set; }
        public float RotateMix { get; set; }
        public float TranslateMix { get; set; }
    }

    public class SP_Event
    {
        public string Name { get; set; }
        public int Int { get; set; }
        public float Float { get; set; }
        public string String { get; set; }
    }
    public class SP_Animation
    {
        public Dictionary<string, SP_AnimationBone> Bones { get; set; }
    }

    public class SP_AnimationBone: Dictionary<string, SP_AnimationFrame[]>
    {
    }

    public class SP_AnimationFrame
    {
        public float? Angle { get; set; }
        public float? X { get; set; }
        public float? Y { get; set; }

        public float[] Curve { get; set; }

        public float Time { get; set; }
    }


    public class SP_Bone
    {
        public string Name { get; set; }

        public float? Length { get; set; }

        public string Parent { get; set; }

        public float? Rotation { get; set; }

        public float X { get; set; }

        public float Y { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
        public float ShearX { get; set; }
        public float ShearY { get; set; }

        public SP_TransformMode TransformMode {  get; set; }
    }

    public class SP_Skeleton
    {
        public string Hash { get; set; }

        public double Height { get; set; }

        public string Images { get; set; }

        public string Spine { get; set; }

        public double Width { get; set; }
    }

    public class SP_Skin
    {
        /// <summary>
        /// 这个是 json 文件读取的
        /// </summary>
        public Dictionary<string, SP_AttachmentCollection> Attachments { get; set; }
        /// <summary>
        /// 从 skel binary 文件解析的
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, SP_AttachmentBase> AttachmentItems { get; set; }

        public string Name { get; set; }


    }

    public class SP_AttachmentCollection : Dictionary<string, SP_AttachmentObject> 
    {

    }

    public class SP_AttachmentObject
    {
        public float Height { get; set; }

        public float? Rotation { get; set; }

        public float Width { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public long[]? Edges { get; set; }

        public long Hull { get; set; }

        public long[]? Triangles { get; set; }

        public string Type { get; set; }

        public double[]? Uvs { get; set; }

        public double[]? Vertices { get; set; }

    }

    public abstract class SP_AttachmentBase
    {
        public string Name { get; set; }
    }
    public class SP_VertexAttachment : SP_AttachmentBase
    {
        public int Id { get; set; }
        public int[] Bones { get; set; }
        public float[] Vertices { get; set; }
        public int WorldVerticesLength { get; set; }
    }

    public class SP_BoundingBoxAttachment : SP_VertexAttachment
    {

    }

    public class SP_ClippingAttachment : SP_VertexAttachment
    {
        public string EndSlot { get; set; }
    }

    public class SP_MeshAttachment : SP_VertexAttachment
    {
        public int HullLength { get; set; }
        public float[] RegionUVs { get; set; }
        public float[] UVs { get; set; }
        public int[] Triangles { get; set; }

        public SKColor Color { get; set; }

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
    }

    public class SP_PathAttachment : SP_VertexAttachment
    {
        public float[] Lengths { get; set; }
        public bool Closed { get; set; }
        public bool ConstantSpeed { get; set; }
    }

    public class SP_PointAttachment : SP_AttachmentBase
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
    }

    public class SP_RegionAttachment : SP_AttachmentBase
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

        public SKColor Color { get; set; }

        public string Path { get; set; }
        public object RendererObject { get; set; }
        public float RegionOffsetX { get; set; }
        public float RegionOffsetY { get; set; }
        public float RegionWidth { get; set; }
        public float RegionHeight { get; set; }
        public float RegionOriginalWidth { get; set; }
        public float RegionOriginalHeight { get; set; }

        public float[] Offset { get; set; }
        public float[] UVs { get; set; }
    }

    public class SP_Slot
    {
        public string Attachment { get; set; }

        public string Bone { get; set; }

        public string Name { get; set; }
    }

    [Flags]
    public enum SP_TransformMode
    {
        //0000 0 Flip Scale Rotation
        Normal = 0, // 0000
        OnlyTranslation = 7, // 0111
        NoRotationOrReflection = 1, // 0001
        NoScale = 2, // 0010
        NoScaleOrReflection = 6, // 0110
    }

    public enum SP_BlendMode
    {
        Normal, 
        Additive, 
        Multiply, 
        Screen
    }

    public enum SP_PositionMode
    {
        Fixed, Percent
    }

    public enum SP_SpacingMode
    {
        Length, Fixed, Percent
    }

    public enum SP_RotateMode
    {
        Tangent, Chain, ChainScale
    }

    public enum SP_AttachmentType
    {
        Region, BoundingBox, Mesh, LinkedMesh, Path, Point, Clipping
    }
}
