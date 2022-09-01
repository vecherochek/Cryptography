using System.Collections;

namespace Cryptography.Extensions
{
    public static class BitArrayExtensions
    {
        public static BitArray BitsPermutation(BitArray block, byte[] permutationTable)
        {
            var changed = new BitArray(permutationTable.Length);
            for (var i = 0; i < permutationTable.Length; i++)
            {
                changed[i] = block[permutationTable[i] - 1];
            }

            return changed;
        }

        public static BitArray BitsConcat(this BitArray left, BitArray right)
        {
            var changed = new BitArray(left.Count + right.Count);
            for (var i = 0; i < left.Count; i++)
            {
                changed[i] = left[i];
            }

            for (var i = 0; i < right.Count; i++)
            {
                changed[i + left.Count] = right[i];
            }

            return changed;
        }
    }
}