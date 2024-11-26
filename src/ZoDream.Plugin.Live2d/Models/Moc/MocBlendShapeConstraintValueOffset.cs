﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Live2d.Models
{
    internal class MocBlendShapeConstraintValueOffsetPtr
    {
        public uint Keys {  get; private set; }
        public uint Weights {  get; private set; }

        public void Read(BinaryReader reader)
        {
            Keys = reader.ReadUInt32();
            Weights = reader.ReadUInt32();
        }
    }
    internal class MocBlendShapeConstraintValueOffset
    {
        public float[] Keys { get; private set; }
        public float[] Weights { get; private set; }

        public void Read(BinaryReader reader, int count)
        {
            var ptr = new MocBlendShapeConstraintValueOffsetPtr();
            ptr.Read(reader);
            var pos = reader.BaseStream.Position;

            Keys = reader.ReadArray(ptr.Keys, count, () => reader.ReadSingle());
            Weights = reader.ReadArray(ptr.Weights, count, () => reader.ReadSingle());

            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        }
    }
}
