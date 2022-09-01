using SymmetricalAlgorithm;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace DES
{
    public class DESFeistelFunction : IEncryptionTransformation
    {
        public byte[] Transform(byte[] block, byte[] roundKey)
        {
            var expandingPermutation = Permutation(block, Tables.ExpandingPermutation);
            var xor = expandingPermutation.Xor(roundKey);
            var sBlock = PermutationSBlock(xor, Tables.SBlocks);

            return Permutation(sBlock, Tables.PBlock);
        }
    }
}