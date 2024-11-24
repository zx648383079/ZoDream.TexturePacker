using SkiaSharp;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZoDream.Plugin.Spine.Models;
using ZoDream.Shared.Models;

namespace ZoDream.Plugin.Spine
{
    /// <summary>
    /// skel 文件读取
    /// </summary>
    public partial class SkeletonReader
    {
        public const int BONE_ROTATE = 0;
        public const int BONE_TRANSLATE = 1;
        public const int BONE_SCALE = 2;
        public const int BONE_SHEAR = 3;

        public const int SLOT_ATTACHMENT = 0;
        public const int SLOT_COLOR = 1;
        public const int SLOT_TWO_COLOR = 2;

        public const int PATH_POSITION = 0;
        public const int PATH_SPACING = 1;
        public const int PATH_MIX = 2;

        public const int CURVE_LINEAR = 0;
        public const int CURVE_STEPPED = 1;
        public const int CURVE_BEZIER = 2;

        private float Scale = 1;


        public IEnumerable<SkeletonSection>? Read(Stream input)
        {
            var res = new SkeletonRoot()
            {
                Skeleton = new()
            };
            var reader = new BinaryReader(input);
            res.Skeleton.Hash = ReadString(reader);
            res.Skeleton.Version = ReadString(reader);
            res.Skeleton.Width = ReadSingle(reader);
            res.Skeleton.Height = ReadSingle(reader);
            var nonessential = reader.ReadBoolean();
            if (nonessential)
            {
                res.Skeleton.Fps = ReadSingle(reader);
                res.Skeleton.ImagesPath = ReadString(reader);
            }
            // Bones
            res.Bones = ReadArray(reader, i => {
                var bone = new Bone
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
                    TransformMode = (TransformMode)reader.Read7BitEncodedInt()
                };
                if (nonessential)
                {
                    ReadColor(reader); // Skip bone color.
                }
                return bone;
            }); 
            // Slots.
            res.Slots = ReadArray(reader, _ => {
                var slot = new Slot
                {
                    Name = ReadString(reader),
                    Bone = res.Bones[reader.Read7BitEncodedInt()].Name
                };
                var color = ReadColor(reader);
                var darkColor = ReadColor(reader, false); // 0x00rrggbb
                slot.Attachment = ReadString(reader);
                var blendMode = (BlendMode)reader.Read7BitEncodedInt();
                return slot;
            });
            // IK constraints.
            res.IkConstraints = ReadArray(reader, _ => {
                var ik = new IkConstraint
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
            res.TransformConstraints = ReadArray(reader, _ => {
                var tc = new TransformConstraint
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
            res.PathConstraints = ReadArray(reader, _ => {
                var pc = new PathConstraint
                {
                    Name = ReadString(reader),
                    Order = reader.Read7BitEncodedInt(),
                    Bones = ReadArray(reader, _ => {
                        return res.Bones[reader.Read7BitEncodedInt()].Name;
                    }),
                    Target = res.Bones[reader.Read7BitEncodedInt()].Name,
                    PositionMode = (PositionMode)reader.Read7BitEncodedInt(),
                    SpacingMode = (SpacingMode)reader.Read7BitEncodedInt(),
                    RotateMode = (RotateMode)reader.Read7BitEncodedInt(),
                    OffsetRotation = ReadSingle(reader),
                    Position = ReadSingle(reader)
                };
                if (pc.PositionMode == PositionMode.Fixed)
                {
                    pc.Position *= Scale;
                }
                pc.Spacing = ReadSingle(reader);
                if (pc.SpacingMode == SpacingMode.Fixed || pc.SpacingMode == SpacingMode.Length)
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
            res.Events = ReadArray(reader, _ => {
                var e = new Event()
                {
                    Name = ReadString(reader),
                    Int  = reader.Read7BitEncodedInt(),
                    Float = ReadSingle(reader),
                    String = ReadString(reader)
                };
                return e;
            });
            // Animations.
            res.Animations = ReadArray(reader, _ => {
                return ReadAnimation(reader, res);
            });
            return [res.ToSkeleton()];
        }

        private Skin ReadSkin(BinaryReader reader, string name, 
            SkeletonRoot res, bool nonessential)
        {
            var slotCount = reader.Read7BitEncodedInt();
            if (slotCount <= 0)
            {
                return null;
            }
            var skin = new Skin
            {
                Name = name,
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
                    skin.Add(slotIndex, name, attachment);
                });
            });
            return skin;
        }

        private AttachmentBase ReadAttachment(BinaryReader reader, SkeletonRoot res, Skin skin, int slotIndex, string attachmentName, bool nonessential)
        {
            var name = ReadString(reader);
            var type = (AttachmentType)reader.ReadByte();
            switch (type)
            {
                case AttachmentType.Region:
                    return new RegionAttachment()
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
                case AttachmentType.BoundingBox:
                    var b = new BoundingBoxAttachment()
                    {
                        Name = name
                    };
                    ReadVertices(reader, b);
                    if (nonessential)
                    {
                        ReadColor(reader);
                    }
                    return b;
                case AttachmentType.Mesh:
                    var m = new MeshAttachment()
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
                case AttachmentType.LinkedMesh:
                    var l = new MeshAttachment()
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
                case AttachmentType.Path:
                    var p = new PathAttachment()
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
                case AttachmentType.Point:
                    var t = new PointAttachment()
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
                case AttachmentType.Clipping:
                    var c = new ClippingAttachment()
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

        private Animation ReadAnimation(BinaryReader reader, SkeletonRoot res)
        {
            var name = ReadString(reader);
            var timelines = new List<Timeline>();
            var duration = 0f;

            // Slot timelines.
            ReadArray(reader, _ => {
                var slotIndex = reader.Read7BitEncodedInt();
                ReadArray(reader, _ => {
                    var timelineType = reader.ReadByte();
                    var frameCount = reader.Read7BitEncodedInt();
                    switch (timelineType)
                    {
                        case SLOT_ATTACHMENT:
                            {
                                var timeline = new AttachmentTimeline(frameCount)
                                {
                                    SlotIndex = slotIndex,
                                };
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.AttachmentNames[i] = ReadString(reader);
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
                                break;
                            }
                        case SLOT_COLOR:
                            {
                                var timeline = new ColorTimeline(frameCount)
                                {
                                    SlotIndex = slotIndex
                                };
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.ColorFrames[i] = ReadColor(reader);
                                    if (i < frameCount - 1)
                                    {
                                        ReadCurve(reader, i, timeline);
                                    }
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[timeline.FrameCount - 1]);
                                break;
                            }
                        case SLOT_TWO_COLOR:
                            {
                                var timeline = new TwoColorTimeline(frameCount)
                                {
                                    SlotIndex = slotIndex,
                                };
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.ColorFrames[i] = ReadColor(reader);
                                    timeline.Color2Frames[i] = ReadColor(reader, false);
                                    if (i < frameCount - 1)
                                    {
                                        ReadCurve(reader, i, timeline);
                                    }
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[timeline.FrameCount - 1]);
                                break;
                            }
                    }
                });
            });

            // Bone timelines.
            ReadArray(reader, _ => {
                var boneIndex = reader.Read7BitEncodedInt();
                ReadArray(reader, _ => {
                    var timelineType = reader.ReadByte();
                    var frameCount = reader.Read7BitEncodedInt();
                    switch (timelineType)
                    {
                        case BONE_ROTATE:
                            {
                                var timeline = new RotateTimeline(frameCount)
                                {
                                    BoneIndex = boneIndex,
                                };
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.AngleItems[i] = ReadSingle(reader);
                                    if (i < frameCount - 1)
                                    {
                                        ReadCurve(reader, i, timeline);
                                    }
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
                                break;
                            }
                        case BONE_TRANSLATE:
                        case BONE_SCALE:
                        case BONE_SHEAR:
                            {
                                TranslateTimeline timeline;
                                var timelineScale = 1f;
                                if (timelineType == BONE_SCALE)
                                    timeline = new ScaleTimeline(frameCount);
                                else if (timelineType == BONE_SHEAR)
                                    timeline = new ShearTimeline(frameCount);
                                else
                                {
                                    timeline = new TranslateTimeline(frameCount);
                                    timelineScale = Scale;
                                }
                                timeline.BoneIndex = boneIndex;
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.Points[i] = new SKPoint(ReadSingle(reader) * timelineScale, ReadSingle(reader) * timelineScale);
                                  
                                    if (i < frameCount - 1)
                                    {
                                        ReadCurve(reader, i, timeline);
                                    }
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
                                break;
                            }
                    }
                });
            });

            // IK timelines.
            ReadArray(reader, _ => {
                var index = reader.Read7BitEncodedInt();
                var frameCount = reader.Read7BitEncodedInt();
                var timeline = new IkConstraintTimeline(frameCount)
                {
                    IkConstraintIndex = index,
                };
                ReadArray(frameCount, i => {
                    timeline.Frames[i] = ReadSingle(reader);
                    timeline.MixItems[i] = ReadSingle(reader);
                    timeline.DirectionItems[i] = reader.ReadByte();
                    if (i < frameCount - 1)
                    {
                        ReadCurve(reader, i, timeline);
                    }
                });
                timelines.Add(timeline);
                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
            });

            // Transform constraint timelines.
            ReadArray(reader, _ => {
                var index = reader.Read7BitEncodedInt();
                var frameCount = reader.Read7BitEncodedInt();
                var timeline = new TransformConstraintTimeline(frameCount)
                {
                    TransformConstraintIndex = index
                };
                ReadArray(frameCount, i => {
                    timeline.Frames[i] = ReadSingle(reader);
                    timeline.RotateItems[i] = ReadSingle(reader);
                    timeline.TranslateItems[i] = ReadSingle(reader);
                    timeline.ScaleItems[i] = ReadSingle(reader);
                    timeline.ShearItems[i] = ReadSingle(reader);
                    if (i < frameCount - 1)
                    {
                        ReadCurve(reader, i, timeline);
                    }
                });
                timelines.Add(timeline);
                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
            });

            // Path constraint timelines.
            ReadArray(reader, _ => {
                var index = reader.Read7BitEncodedInt();
                var data = res.PathConstraints[index];
                ReadArray(reader, _ => {
                    var timelineType = reader.ReadByte();
                    var frameCount = reader.Read7BitEncodedInt();
                    switch (timelineType)
                    {
                        case PATH_POSITION:
                        case PATH_SPACING:
                            {
                                PathConstraintPositionTimeline timeline;
                                float timelineScale = 1;
                                if (timelineType == PATH_SPACING)
                                {
                                    timeline = new PathConstraintSpacingTimeline(frameCount);
                                    if (data.SpacingMode == SpacingMode.Length || data.SpacingMode == SpacingMode.Fixed)
                                    {
                                        timelineScale = Scale;
                                    }
                                }
                                else
                                {
                                    timeline = new PathConstraintPositionTimeline(frameCount);
                                    if (data.PositionMode == PositionMode.Fixed)
                                    {
                                        timelineScale = Scale;
                                    }
                                }
                                timeline.PathConstraintIndex = index;
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.PositionItems[i] = ReadSingle(reader) * timelineScale;
                                    if (i < frameCount - 1)
                                    {
                                        ReadCurve(reader, i, timeline);
                                    }
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
                                break;
                            }
                        case PATH_MIX:
                            {
                                var timeline = new PathConstraintMixTimeline(frameCount)
                                {
                                    PathConstraintIndex = index,
                                };
                                ReadArray(frameCount, i => {
                                    timeline.Frames[i] = ReadSingle(reader);
                                    timeline.RotateItems[i] = ReadSingle(reader);
                                    timeline.TranslateItems[i] = ReadSingle(reader);
                                    if (i < frameCount - 1)
                                    {
                                        ReadCurve(reader, i, timeline);
                                    }
                                });
                                timelines.Add(timeline);
                                duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
                                break;
                            }
                    }
                });
            });

            // Deform timelines.
            ReadArray(reader, _ => {
                var skinIndex = reader.Read7BitEncodedInt();
                var skin = res.Skins[skinIndex];
                ReadArray(reader, _ => {
                    var slotIndex = reader.Read7BitEncodedInt();
                    ReadArray(reader, _ => {
                        var attachmentName = ReadString(reader);
                        skin.TryGet<VertexAttachment>(slotIndex, attachmentName, out var attachment);
                        var weighted = attachment!.Bones != null;
                        float[] vertices = attachment.Vertices;
                        var deformLength = weighted ? vertices.Length / 3 * 2 : vertices.Length;

                        var frameCount = reader.Read7BitEncodedInt();
                        var timeline = new DeformTimeline(frameCount)
                        {
                            SlotIndex = slotIndex,
                            Attachment = attachment,
                        };

                        ReadArray(frameCount, i => {
                            timeline.Frames[i] = ReadSingle(reader);
                            float[] deform;
                            int end = reader.Read7BitEncodedInt();
                            if (end == 0)
                                deform = weighted ? new float[deformLength] : vertices;
                            else
                            {
                                deform = new float[deformLength];
                                int start = reader.Read7BitEncodedInt();
                                end += start;
                                for (int v = start; v < end; v++)
                                {
                                    deform[v] = ReadSingle(reader) * Scale;
                                }
                                if (!weighted)
                                {
                                    for (int v = 0, vn = deform.Length; v < vn; v++)
                                    {
                                        deform[v] += vertices[v];
                                    }
                                }
                            }
                            timeline.Vertices[i] = deform;
                            if (i < frameCount - 1)
                            {
                                ReadCurve(reader, i, timeline);
                            }
                        });
                        timelines.Add(timeline);
                        duration = Math.Max(duration, timeline.Frames[frameCount - 1]);
                    });
                });
            });

            // Draw order timeline.
            int drawOrderCount = reader.Read7BitEncodedInt();
            if (drawOrderCount > 0)
            {
                var timeline = new DrawOrderTimeline(drawOrderCount);
                var slotCount = res.Slots.Length;
                ReadArray(drawOrderCount, i => {
                    timeline.Frames[i] = ReadSingle(reader);
                    int offsetCount = reader.Read7BitEncodedInt();
                    int[] drawOrder = new int[slotCount];
                    for (int ii = slotCount - 1; ii >= 0; ii--)
                    {
                        drawOrder[ii] = -1;
                    }
                    int[] unchanged = new int[slotCount - offsetCount];
                    int originalIndex = 0, unchangedIndex = 0;
                    for (int ii = 0; ii < offsetCount; ii++)
                    {
                        int slotIndex = reader.Read7BitEncodedInt();
                        // Collect unchanged items.
                        while (originalIndex != slotIndex)
                            unchanged[unchangedIndex++] = originalIndex++;
                        // Set changed items.
                        drawOrder[originalIndex + reader.Read7BitEncodedInt()] = originalIndex++;
                    }
                    // Collect remaining unchanged items.
                    while (originalIndex < slotCount)
                        unchanged[unchangedIndex++] = originalIndex++;
                    // Fill in unchanged items.
                    for (int ii = slotCount - 1; ii >= 0; ii--)
                        if (drawOrder[ii] == -1) drawOrder[ii] = unchanged[--unchangedIndex];
                    timeline.DrawOrders[i] = drawOrder;
                });
                timelines.Add(timeline);
                duration = Math.Max(duration, timeline.Frames[drawOrderCount - 1]);
            }

            // Event timeline.
            int eventCount = reader.Read7BitEncodedInt();
            if (eventCount > 0)
            {
                var timeline = new EventTimeline(eventCount);
                ReadArray(eventCount, i => {
                    timeline.Frames[i] = ReadSingle(reader);
                    var eventIndex = reader.Read7BitEncodedInt();
                    var eventData = res.Events[eventIndex];
                    
                    var e = new Event
                    {
                        Name = eventData.Name,
                        Int = reader.Read7BitEncodedInt(),
                        Float = ReadSingle(reader),
                        String = reader.ReadBoolean() ? ReadString(reader) : eventData.String
                    };
                    timeline.Events[i] = e;
                });
                timelines.Add(timeline);
                duration = Math.Max(duration, timeline.Frames[eventCount - 1]);
            }

            timelines.TrimExcess();
            return new Animation()
            {
                Name = name,
                Timelines = [.. timelines],
                Duration = duration,
            };
        }

        private void ReadCurve(BinaryReader reader, int frameIndex, CurveTimeline timeline)
        {
            switch (reader.ReadByte())
            {
                case CURVE_STEPPED:
                    timeline.SetStepped(frameIndex);
                    break;
                case CURVE_BEZIER:
                    timeline.SetCurve(frameIndex, 
                        ReadSingle(reader), ReadSingle(reader), 
                        ReadSingle(reader), ReadSingle(reader));
                    break;
            }
        }

        private void ReadVertices(BinaryReader reader, VertexAttachment res)
        {
            var vertexCount = reader.Read7BitEncodedInt();
            res.WorldVerticesLength = vertexCount << 1;
            if (res is MeshAttachment m)
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
            if (res is PathAttachment p)
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

        private SKColor ReadColor(BinaryReader reader, bool hasAlpha = true)
        {
            if (!hasAlpha)
            {
                _ = reader.ReadByte();
            }
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();
            var a = hasAlpha ? reader.ReadByte() : byte.MaxValue;
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
