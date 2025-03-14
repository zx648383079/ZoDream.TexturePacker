using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ZoDream.Plugin.Live2d
{
    internal unsafe static partial class NativeMethods
    {
        static NativeMethods()
        {
            NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, DllImportResolver);
        }

        static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            var entry = Path.Combine(AppContext.BaseDirectory,
                    RuntimeInformation.ProcessArchitecture switch
                    {
                        Architecture.X86 or Architecture.X64 or Architecture.Arm64 =>
                            Enum.GetName(RuntimeInformation.ProcessArchitecture).ToLower(),
                        _ => throw new NotImplementedException(),
                    },
                    DllName + ".dll");
            return NativeLibrary.Load(entry, assembly, searchPath);
        }
    }
}
