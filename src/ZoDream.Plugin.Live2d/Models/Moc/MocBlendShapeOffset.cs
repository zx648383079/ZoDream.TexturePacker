﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Live2d.Models
{
    internal class MocBlendShapeOffsetPtr
    {
        public uint TargetIndices {  get; private set; }
        public uint BlendShapeKeyFormBindingSourcesBeginIndices {  get; private set; }
        public uint BlendShapeKeyFormBindingSourcesCounts {  get; private set; }
        
        public void Read(BinaryReader reader)
        {
            TargetIndices = reader.ReadUInt32();
            BlendShapeKeyFormBindingSourcesBeginIndices = reader.ReadUInt32();
            BlendShapeKeyFormBindingSourcesCounts = reader.ReadUInt32();
        }
    }
    internal class MocBlendShapeOffset
    {
        public int[] TargetIndices { get; private set; }
        public int[] BlendShapeKeyFormBindingSourcesBeginIndices { get; private set; }
        public int[] BlendShapeKeyFormBindingSourcesCounts { get; private set; }

        public void Read(BinaryReader reader, int count)
        {
            var ptr = new MocBlendShapeOffsetPtr();
            ptr.Read(reader);
            var pos = reader.BaseStream.Position;

            TargetIndices = reader.ReadArray(ptr.TargetIndices, count, () => reader.ReadInt32());
            BlendShapeKeyFormBindingSourcesBeginIndices = reader.ReadArray(ptr.BlendShapeKeyFormBindingSourcesBeginIndices, count, () => reader.ReadInt32());
            BlendShapeKeyFormBindingSourcesCounts = reader.ReadArray(ptr.BlendShapeKeyFormBindingSourcesCounts, count, () => reader.ReadInt32());

            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        }
    }
}
