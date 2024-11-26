using System;
using System.IO;
using System.Text;

namespace ZoDream.Plugin.Live2d
{
    internal static class IOExtension
    {
        public static T[] ReadArray<T>(this BinaryReader reader, int length, Func<T> cb)
        {
            if (length == 0)
            {
                return [];
            }
            var items = new T[length];
            for (var i = 0; i < length; i++)
            {
                items[i] = cb.Invoke();
            }
            return items;
        }

        public static T[] ReadArray<T>(this BinaryReader reader, uint ptr, int length, Func<T> cb)
        {
            reader.BaseStream.Seek(ptr, SeekOrigin.Begin);
            return ReadArray(reader, length, cb);
        }

        public static string ConvertString(byte[] buffer)
        {
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                if (buffer[i] != 0x0)
                {
                    return Encoding.UTF8.GetString(buffer, 0, i + 1);
                }
            }
            return string.Empty;
        }
    }
}
