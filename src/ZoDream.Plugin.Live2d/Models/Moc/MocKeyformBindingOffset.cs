﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Plugin.Live2d.Models
{
    internal class MocKeyFormBindingOffsetPtr
    {
        public uint ParameterBindingIndexSourcesBeginIndices {  get; private set; }
        public uint ParameterBindingIndexSourcesCounts {  get; private set; }

        public void Read(BinaryReader reader)
        {
            ParameterBindingIndexSourcesBeginIndices = reader.ReadUInt32();
            ParameterBindingIndexSourcesCounts = reader.ReadUInt32();
        }
    }
    internal class MocKeyFormBindingOffset
    {
        public int[] ParameterBindingIndexSourcesBeginIndices { get; private set; }
        public int[] ParameterBindingIndexSourcesCounts { get; private set; }

        public void Read(BinaryReader reader, int count)
        {
            var ptr = new MocKeyFormBindingOffsetPtr();
            ptr.Read(reader);
            var pos = reader.BaseStream.Position;

            ParameterBindingIndexSourcesBeginIndices = reader.ReadArray(ptr.ParameterBindingIndexSourcesBeginIndices, count, () => reader.ReadInt32());
            ParameterBindingIndexSourcesCounts = reader.ReadArray(ptr.ParameterBindingIndexSourcesCounts, count, () => reader.ReadInt32());
           

            reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        }
    }
}
