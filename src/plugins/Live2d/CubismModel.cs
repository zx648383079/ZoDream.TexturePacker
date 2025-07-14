using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ZoDream.Plugin.Live2d
{
    public class CubismModel : IDisposable
    {
        //Alignment constraints.

        /// <summary>
        /// Necessary alignment for mocs (in bytes).
        /// </summary>
        const int csmAlignofMoc = 64;
        /// <summary>
        /// Necessary alignment for models (in bytes).
        /// </summary>
        const int CsmAlignofModel = 16;

        //Bit masks for non-dynamic drawable flags.

        /// <summary>
        /// Additive blend mode mask.
        /// </summary>
        const byte CsmBlendAdditive = 1 << 0;
        /// <summary>
        /// Multiplicative blend mode mask.
        /// </summary>
        const byte CsmBlendMultiplicative = 1 << 1;
        /// <summary>
        /// Double-sidedness mask.
        /// </summary>
        const byte CsmIsDoubleSided = 1 << 2;
        /// <summary>
        /// Clipping mask inversion mode mask.
        /// </summary>
        const byte CsmIsInvertedMask = 1 << 3;

        //Bit masks for dynamic drawable flags.

        /// <summary>
        /// Flag set when visible.
        /// </summary>
        const byte CsmIsVisible = 1 << 0;
        /// <summary>
        /// Flag set when visibility did change.
        /// </summary>
        const byte CsmVisibilityDidChange = 1 << 1;
        /// <summary>
        /// Flag set when opacity did change.
        /// </summary>
        const byte CsmOpacityDidChange = 1 << 2;
        /// <summary>
        /// Flag set when draw order did change.
        /// </summary>
        const byte CsmDrawOrderDidChange = 1 << 3;
        /// <summary>
        /// Flag set when render order did change.
        /// </summary>
        const byte CsmRenderOrderDidChange = 1 << 4;
        /// <summary>
        /// Flag set when vertex positions did change.
        /// </summary>
        const byte CsmVertexPositionsDidChange = 1 << 5;
        /// <summary>
        /// Flag set when blend color did change.
        /// </summary>
        const byte CsmBlendColorDidChange = 1 << 6;

        //moc3 file format version.

        /// <summary>
        /// unknown
        /// </summary>
        const int CsmMocVersion_Unknown = 0;
        /// <summary>
        /// moc3 file version 3.0.00 - 3.2.07
        /// </summary>
        const int CsmMocVersion_30 = 1;
        /// <summary>
        /// moc3 file version 3.3.00 - 3.3.03
        /// </summary>
        const int CsmMocVersion_33 = 2;
        /// <summary>
        /// moc3 file version 4.0.00 - 4.1.05
        /// </summary>
        const int CsmMocVersion_40 = 3;
        /// <summary>
        /// moc3 file version 4.2.00 -
        /// </summary>
        const int CsmMocVersion_42 = 4;

        //Parameter types.

        /// <summary>
        /// Normal parameter.
        /// </summary>
        const int CsmParameterType_Normal = 0;
        /// <summary>
        /// Parameter for blend shape.
        /// </summary>
        const int CsmParameterType_BlendShape = 1;

        public CubismModel(byte[] mocBytes)
        {
            IntPtr alignedBuffer = AllocateAligned(mocBytes.Length, csmAlignofMoc);
            Marshal.Copy(mocBytes, 0, alignedBuffer, mocBytes.Length);
            _mocPtr = NativeMethods.ReviveMocInPlace(alignedBuffer, mocBytes.Length);
            var modelSize = NativeMethods.GetSizeofModel(_mocPtr);
            var modelMemory = AllocateAligned(modelSize, CsmAlignofModel);

            _modelPtr = NativeMethods.InitializeModelInPlace(_mocPtr, modelMemory, modelSize);
        }

        private readonly IntPtr _mocPtr;
        private readonly IntPtr _modelPtr;


        #region 方法
        public Vector2 GetCanvasSize()
        {
            NativeMethods.ReadCanvasInfo(_modelPtr, out var tmpSizeInPixels, out _, out var tmpPixelsPerUnit);
            return tmpSizeInPixels / tmpPixelsPerUnit;
        }

        public unsafe int GetDrawableVertexCount(int drawableIndex)
        {
            return NativeMethods.GetDrawableVertexCounts(_modelPtr)[drawableIndex];
        }

        public unsafe Vector2* GetDrawableVertexPositions(int drawableIndex)
        {
            return NativeMethods.GetDrawableVertexPositions(_modelPtr)[drawableIndex];
        }

        public unsafe Vector2* GetDrawableVertexUvs(int drawableIndex)
        {
            return NativeMethods.GetDrawableVertexUvs(_modelPtr)[drawableIndex];
        }


        public void Update()
        {
            // Update model.
            NativeMethods.UpdateModel(_modelPtr);
            // Reset dynamic drawable flags.
            NativeMethods.ResetDrawableDynamicFlags(_modelPtr);
        }

        #endregion

        #region 分配内存地址
        private IntPtr Allocate(int size)
        {
            return Marshal.AllocHGlobal(size);
        }

        private void Deallocate(IntPtr memory)
        {
            Marshal.FreeHGlobal(memory);
        }

        private unsafe IntPtr AllocateAligned(int size, int alignment)
        {
            IntPtr offset, shift, alignedAddress;
            IntPtr allocation;
            void** preamble;

            offset = alignment - 1 + sizeof(void*);

            allocation = Allocate((int)(size + offset));

            alignedAddress = allocation + sizeof(void*);

            shift = alignedAddress % alignment;

            if (shift != 0)
            {
                alignedAddress += alignment - shift;
            }

            preamble = (void**)alignedAddress;
            preamble[-1] = (void*)allocation;

            return alignedAddress;
        }

        private unsafe void DeallocateAligned(IntPtr alignedMemory)
        {
            var preamble = (void**)alignedMemory;

            Deallocate(new IntPtr(preamble[-1]));
        }
        #endregion


        public void Dispose()
        {
            DeallocateAligned(_modelPtr);
            DeallocateAligned(_mocPtr);
            GC.SuppressFinalize(this);
        }
    }
}
