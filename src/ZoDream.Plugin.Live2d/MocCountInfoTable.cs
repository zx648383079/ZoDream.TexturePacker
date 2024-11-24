using System.Runtime.InteropServices;

namespace ZoDream.Plugin.Live2d
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
        public uint PartKeyForms;
        public uint WarpDeformerKeyForms;
        public uint RotationDeformerKeyForms;
        public uint ArtMeshKeyForms;
        public uint KeyFormPositions;
        public uint ParameterBindingIndices;
        public uint KeyFormBindings;
        public uint ParameterBindings;
        public uint Keys;
        public uint Uvs;
        public uint PositionIndices;
        public uint DrawableMasks;
        public uint DrawOrderGroups;
        public uint DrawOrderGroupObjects;
        public uint Glue;
        public uint GlueInfo;
        public uint GlueKeyForms;
    }
}
