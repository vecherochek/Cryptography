using System;
using System.Numerics;

namespace DES
{
    public class Functions
    {
        public static byte[] Permutation32(byte[] block, byte[] permutationTable)
        {
            var number = BitConverter.ToUInt32(block, 0);
            ulong changed = 0;
            for (var i = 0; i < permutationTable.Length; ++i)
            {
                var findBit = (number >> (permutationTable[i] - 1)) & 1;
                changed |= findBit << i;
            }

            return BitConverter.GetBytes(changed);
        }
        public static byte[] Permutation64(byte[] block, byte[] permutationTable)
        {
            var number = BitConverter.ToUInt64(block, 0);
            ulong changed = 0;
            for (var i = 0; i < permutationTable.Length; ++i)
            {
                var findBit = (number >> (permutationTable[i] - 1)) & 1;
                changed |= findBit << i;
            }

            return BitConverter.GetBytes(changed);
        }
        public static ulong Permutation64To56(byte[] key, byte[] permutationTable)
        {
            var number = BitConverter.ToUInt64(key, 0);
            ulong changed = 0;
            
            for (var i = 0; i < permutationTable.Length; ++i)
            {
                changed |= ((number >> (permutationTable[i] - 1)) & 1) << i;
            }

            return changed;
        }
    }
}