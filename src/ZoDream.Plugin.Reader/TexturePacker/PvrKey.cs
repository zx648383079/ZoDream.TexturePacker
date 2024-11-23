using System;
using System.Collections.Generic;

namespace ZoDream.Plugin.Readers.TexturePacker
{
    public class PvrKey
    {

        public PvrKey(string key)
        {
            _iv = GenerateEncryptionKey(GenerateKeyParts(key));
        }

        private int[] _iv;

        public void Decrypt(ref byte[] buffer)
        {
            const int enclen = 1024;
            const int securelen = 512 * 4;
            const int distance = 64 * 4;
            var b = 0;
            var i = 0;
            var len = buffer.Length - 12;
            // encrypt first part completely
            for (; i < len && i < securelen; i += 4)
            {
                var key = BitConverter.GetBytes(_iv[b++]);
                buffer[i + 0] ^= key[0];
                buffer[i + 1] ^= key[1];
                buffer[i + 2] ^= key[2];
                buffer[i + 3] ^= key[3];
                if (b >= _iv.Length)
                {
                    b = 0;
                }
            }

            i++;

            // encrypt second section partially
            for (; i < len; i += distance)
            {
                var key = BitConverter.GetBytes(_iv[b++]);
                buffer[i + 0] ^= key[0];
                buffer[i + 1] ^= key[1];
                buffer[i + 2] ^= key[2];
                buffer[i + 3] ^= key[3];

                if (b >= enclen)
                {
                    b = 0;
                }
            }
        }

        private int[] GenerateKeyParts(string key)
        {
            var chunkSize = key.Length / 4;
            var res = new List<int>();
            for (var i = 0; i < key.Length; i += chunkSize)
            {
                res.Add(Convert.ToInt32(key.Substring(i, chunkSize), 16));
            }
            return [..res];
        }

        private int[] GenerateEncryptionKey(int[] items)
        {
            var length = 1024;
            var sUEncryptionKey = new int[length];
            var rounds = 6;
            var sum = 0;
            var z = sUEncryptionKey[length - 1];

            while (true)
            {
                sum = LongToUInt(sum + 0x9e3779b9);
                var e = LongToUInt((sum >> 2) & 3);
                var p = 0;
                for (; p < length - 1; p++)
                {
                    var i = sUEncryptionKey[p + 1];
                    sUEncryptionKey[p] = LongToUInt(sUEncryptionKey[p] + MX(z, i, sum, items, p, e));
                    z = sUEncryptionKey[p];
                }
                int y = sUEncryptionKey[0];
                sUEncryptionKey[length - 1] = LongToUInt(sUEncryptionKey[length - 1] + MX(z, y, sum, items, p, e));
                z = sUEncryptionKey[length - 1];
                rounds -= 1;
                if (rounds == 0)
                {
                    break;
                }
            }

            return sUEncryptionKey;
        }

        public static int LongToUInt(long value)
        {
            if (value > 4294967295)
            {
                return (int)(value & (1 << 32 - 1));
            }
            else
            {
                return (int)value;
            }
        }

        public static int MX(int z, int y, int sum, int[] keys, int p, int e)
        {
            return ((z >> 5 ^ y << 2) + (y >> 3 ^ z << 4)) ^ ((sum ^ y) + (keys[(p & 3) ^ e] ^ z));
        }
    }
}
