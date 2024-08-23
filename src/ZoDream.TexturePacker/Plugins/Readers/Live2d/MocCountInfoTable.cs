using System.Runtime.InteropServices;

namespace ZoDream.TexturePacker.Plugins.Readers.Live2d
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MocCountInfoTable
    {
        public uint Parts;
        public uint Deformers;
        public uint WarpDeformers;
        public uint RotationDeformers;
        public uint ArtMeshes;
        public uint Parameters;
        public uint PartKeyforms;
        public uint WarpDeformerKeyforms;
        public uint RotationDeformerKeyforms;
        public uint ArtMeshKeyforms;
        public uint KeyformPositions;
        public uint ParameterBindingIndices;
        public uint KeyformBindings;
        public uint ParameterBindings;
        public uint Keys;
        public uint Uvs;
        public uint PositionIndices;
        public uint DrawableMasks;
        public uint DrawOrderGroups;
        public uint DrawOrderGroupObjects;
        public uint Glue;
        public uint GlueInfo;
        public uint GlueKeyforms;
    }
}
