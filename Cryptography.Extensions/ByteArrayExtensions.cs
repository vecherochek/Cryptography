using System;

namespace Cryptography.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Xor(this byte[] left, byte[] right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }
            
            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }
            
            if (left.Length != right.Length)
            {
                throw new ArgumentException("Arrays lengths must be equal.");
            }

            for (var i = 0; i < left.Length; i++)
            {
                left[i] ^= right[i];
            }

            return left;
        }
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