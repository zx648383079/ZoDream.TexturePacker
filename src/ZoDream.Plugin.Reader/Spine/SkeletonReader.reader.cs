using SkiaSharp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Readers.Spine
{
    /// <summary>
    /// skel 文件读取
    /// </summary>
    public partial class SkeletonReader
    {
        private float Scale = 1;

        public IEnumerable<SkeletonSection>? Read(Stream input)
        {
            var res = new SP_SkeletonRoot()
            {
                Skeleton = new()
            };
            var reader = new BinaryReader(input);
            res.Skeleton.Hash = ReadString(reader);
            var version = ReadString(reader);
            res.Skeleton.Width = ReadSingle(reader);
            res.Skeleton.Height = ReadSingle(reader);
            var nonessential = reader.ReadBoolean();
            if (nonessential)
            {
                var fps = ReadSingle(reader);
                var imagesPath = ReadString(reader);
            }
            // Bones
            res.Bones = ReadArray(reader, i => {
                var bone = new SP_Bone
                {
                    Name = ReadString(reader),
                    Parent = i == 0 ? string.Empty : res.Bones[reader.Read7BitEncodedInt()].Name,
                    Rotation = ReadSingle(reader),
                    X = ReadSingle(reader) * Scale,
                    Y = ReadSingle(reader) * Scale,
                    ScaleX = ReadSingle(reader),
                    ScaleY = ReadSingle(reader),
                    ShearX = ReadSingle(reader),
                    ShearY = ReadSingle(reader),
                    Length = ReadSingle(reader) * Scale,
                    TransformMode = (SP_TransformMode)reader.Read7BitEncodedInt()
                };
                if (nonessential)
                {
                    ReadColor(reader); // Skip bone color.
                }
                return bone;
            }); 
            // Slots.
            res.Slots = ReadArray(reader, _ => {
                var slot = new SP_Slot
                {
                    Name = ReadString(reader),
                    Bone = res.Bones[reader.Read7BitEncodedInt()].Name
                };
                var color = ReadColor(reader);
                var darkColor = ReadColor(reader); // 0x00rrggbb
                slot.Attachment = ReadString(reader);
                var blendMode = (SP_BlendMode)reader.Read7BitEncodedInt();
                return slot;
            });
            // IK constraints.
            res.IkConstraints = ReadArray(reader, _ => {
                var ik = new SP_IkConstraint
                {
                    Name = ReadString(reader),
                    Order = reader.Read7BitEncodedInt(),
                    Bones = ReadArray(reader, _ => {
                        return res.Bones[reader.Read7BitEncodedInt()].Name;
                    }),
                    Target = res.Bones[reader.Read7BitEncodedInt()].Name,
                    Mix = ReadSingle(reader),
                    BendDirection = reader.ReadByte()
                };
                return ik;
            });
            // Transform constraints.
            ReadArray(reader, _ => {
                var tc = new SP_TransformConstraint
                {
                    Name = ReadString(reader),
                    Order = reader.Read7BitEncodedInt(),
                    Bones = ReadArray(reader, _ => {
                        return res.Bones[reader.Read7BitEncodedInt()].Name;
                    }),
                    Target = res.Bones[reader.Read7BitEncodedInt()].Name,
                    Local = reader.ReadBoolean(),
                    Relative = reader.ReadBoolean(),
                    OffsetRotation = ReadSingle(reader),
                    OffsetX = ReadSingle(reader),
                    OffsetY = ReadSingle(reader),
                    OffsetScaleX = ReadSingle(reader),
                    OffsetScaleY = ReadSingle(reader),
                    OffsetShearY = ReadSingle(reader),
                    RotateMix = ReadSingle(reader),
                    TranslateMix = ReadSingle(reader),
                    ScaleMix = ReadSingle(reader),
                    ShearMix = ReadSingle(reader)
                };
                return tc;
            });
            // Path constraints
            ReadArray(reader, _ => {
                var pc = new SP_PathConstraint
                {
                    Name = ReadString(reader),
                    Order = reader.Read7BitEncodedInt(),
                    Bones = ReadArray(reader, _ => {
                        return res.Bones[reader.Read7BitEncodedInt()].Name;
                    }),
                    Target = res.Bones[reader.Read7BitEncodedInt()].Name,
                    PositionMode = (SP_PositionMode)reader.Read7BitEncodedInt(),
                    SpacingMode = (SP_SpacingMode)reader.Read7BitEncodedInt(),
                    RotateMode = (SP_RotateMode)reader.Read7BitEncodedInt(),
                    OffsetRotation = ReadSingle(reader),
                    Position = ReadSingle(reader)
                };
                if (pc.PositionMode == SP_PositionMode.Fixed)
                {
                    pc.Position *= Scale;
                }
                pc.Spacing = ReadSingle(reader);
                if (pc.SpacingMode == SP_SpacingMode.Fixed || pc.SpacingMode == SP_SpacingMode.Length)
                {
                    pc.Spacing *= Scale;
                }
                pc.RotateMix = ReadSingle(reader);
                pc.TranslateMix = ReadSingle(reader);
                return pc;
            });
            // Default skin.
            var defaultSkin = ReadSkin(reader, "default", res, nonessential);
            // Skins.
            var skinItems = ReadArray(reader, _ => {
                return ReadSkin(reader, ReadString(reader), res, nonessential);
            });
            if (defaultSkin is null)
            {
                res.Skins = skinItems;
            } else
            {
                res.Skins = [defaultSkin, .. skinItems];
            }
            // Events.
            ReadArray(reader, _ => {
                var e = new SP_Event()
                {
                    Name = ReadString(reader),
                    Int  = reader.Read7BitEncodedInt(),
                    Float = ReadSingle(reader),
                    String = ReadString(reader)
                };
                return e;
            });
            // Animations.
            ReadArray(reader, _ => {
                ReadAnimation(reader, res);
            });
            return [SkeletonJsonReader.Convert(res)];
        }

        private SP_Skin ReadSkin(BinaryReader reader, string name, 
            SP_SkeletonRoot res, bool nonessential)
        {
            var slotCount = reader.Read7BitEncodedInt();
            if (slotCount <= 0)
            {
                return null;
            }
            var skin = new SP_Skin
            {
                Name = name,
                AttachmentItems = []
            };
            ReadArray(slotCount, _ => {
                var slotIndex = reader.Read7BitEncodedInt();
                ReadArray(reader, _ => {
                    var name = ReadString(reader);
                    var attachment = ReadAttachment(reader, res, skin, slotIndex, name, nonessential);
                    if (attachment is null)
                    {
                        return;
                    }
                    skin.AttachmentItems.Add(name, attachment);
                });
            });
            return skin;
        }

        private SP_AttachmentBase ReadAttachment(BinaryReader reader, SP_SkeletonRoot res, SP_Skin skin, int slotIndex, string attachmentName, bool nonessential)
        {
            var name = ReadString(reader);
            var type = (SP_AttachmentType)reader.ReadByte();
            switch (type)
            {
                case SP_AttachmentType.Region:
                    return new SP_RegionAttachment()
                    {
                        Name = name,
                        Path = ReadString(reader) ?? name,
                        Rotation = ReadSingle(reader),
                        X = ReadSingle(reader),
                        Y = ReadSingle(reader),
                        ScaleX = ReadSingle(reader),
                        ScaleY = ReadSingle(reader),
                        Width = ReadSingle(reader),
                        Height = ReadSingle(reader),
                        Color = ReadColor(reader),
                    };
                case SP_AttachmentType.BoundingBox:
                    var b = new SP_BoundingBoxAttachment()
                    {
                        Name = name
                    };
                    ReadVertices(reader, b);
                    if (nonessential)
                    {
                        ReadColor(reader);
                    }
                    return b;
                case SP_AttachmentType.Mesh:
                    var m = new SP_MeshAttachment()
                    {
                        Name = name,
                        Path = ReadString(reader),
                        Color = ReadColor(reader),
                    };
                    ReadVertices(reader, m);
                    m.HullLength = reader.Read7BitEncodedInt();
                    if (nonessential)
                    {
                        m.Edges = ReadArray(reader, _ => (int)ReadShort(reader));
                        m.Width = ReadSingle(reader);
                        m.Height = ReadSingle(reader);
                    }
                    return m;
                case SP_AttachmentType.LinkedMesh:
                    var l = new SP_MeshAttachment()
                    {
                        Name = name,
                        Path = ReadString(reader),
                        Color = ReadColor(reader),
                    };
                    var skinName = ReadString(reader);
                    var parent = ReadString(reader);
                    l.InheritDeform = reader.ReadBoolean();
                    if (nonessential)
                    {
                        l.Width = ReadSingle(reader);
                        l.Height = ReadSingle(reader);
                    }
                    return l;
                case SP_AttachmentType.Path:
                    var p = new SP_PathAttachment()
                    {
                        Name = name,
                        Closed = reader.ReadBoolean(),
                        ConstantSpeed = reader.ReadBoolean(),
                    };
                    ReadVertices(reader, p);
                    if (nonessential)
                    {
                        ReadColor(reader);
                    }
                    return p;
                case SP_AttachmentType.Point:
                    var t = new SP_PointAttachment()
                    {
                        Rotation = ReadSingle(reader),
                        X = ReadSingle(reader),
                        Y = ReadSingle(reader),
                    };
                    if (nonessential)
                    {
                        ReadColor(reader);
                    }
                    return t;
                case SP_AttachmentType.Clipping:
                    var c = new SP_ClippingAttachment()
                    {
                        Name = name,
                        EndSlot = res.Slots[reader.Read7BitEncodedInt()].Name,

                    };
                    ReadVertices(reader, c);
                    if (nonessential)
                    {
                        ReadColor(reader);
                    }
                    return c;
                default:
                    return null;
            }
        }

        private void ReadAnimation(BinaryReader reader, SP_SkeletonRoot res)
        {

        }

        private void ReadVertices(BinaryReader reader, SP_VertexAttachment res)
        {
            var vertexCount = reader.Read7BitEncodedInt();
            res.WorldVerticesLength = vertexCount << 1;
            if (res is SP_MeshAttachment m)
            {
                m.UVs = ReadArray(res.WorldVerticesLength, _ => {
                    return ReadSingle(reader);
                });
                m.Triangles = ReadArray(reader, _ => {
                    return (int)ReadShort(reader);
                });
            }

            if (!reader.ReadBoolean())
            {
                res.Vertices = ReadArray(res.WorldVerticesLength, _ => {
                    return ReadSingle(reader) * Scale;
                });
            } else
            {
                var weights = new List<float>();
                var bones = new List<int>();
                ReadArray(vertexCount, _ => {
                    var boneCount = reader.Read7BitEncodedInt();
                    bones.Add(boneCount);
                    ReadArray(boneCount, _ => {
                        bones.Add(reader.Read7BitEncodedInt());
                        weights.Add(ReadSingle(reader) * Scale);
                        weights.Add(ReadSingle(reader) * Scale);
                        weights.Add(ReadSingle(reader) * Scale);
                    });
                });
                res.Vertices = [.. weights];
                res.Bones = [.. bones];
            }
            if (res is SP_PathAttachment p)
            {
                p.Lengths = ReadArray(vertexCount / 3, _ => {
                    return ReadSingle(reader) * Scale;
                });
            }
        }

        private void ReadArray(BinaryReader reader, Action<int> cb)
        {
            ReadArray(reader.Read7BitEncodedInt(), cb);
        }

        private void ReadArray(int length, Action<int> cb)
        {
            for (var i = 0; i < length; i++)
            {
                cb.Invoke(i);
            }
        }
        private T[] ReadArray<T>(BinaryReader reader, Func<int, T> cb)
        {
            return ReadArray(reader.Read7BitEncodedInt(), cb);
        }

        private T[] ReadArray<T>(int length, Func<int, T> cb)
        {
            var items = new T[length];
            for (var i = 0; i < length; i++)
            {
                items[i] = cb.Invoke(i);
            }
            return items;
        }

        private float ReadSingle(BinaryReader reader)
        {
            return BinaryPrimitives.ReadSingleBigEndian(reader.ReadBytes(4));
        }

        private float ReadShort(BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt16BigEndian(reader.ReadBytes(2));
        }

        private SKColor ReadColor(BinaryReader reader)
        {
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();
            var a = reader.ReadByte();
            return new SKColor(r, g, b, a);
        }

        private int ReadInt(BinaryReader reader)
        {
            return BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));
        }

        private string ReadString(BinaryReader reader)
        {
            var length = reader.Read7BitEncodedInt();
            if (length < 2)
            {
                return string.Empty;
            }
            return Encoding.UTF8.GetString(reader.ReadBytes(length - 1));
        }
    }
}
