using System;
using static Cryptography.Extensions.ByteArrayExtensions;
using SymmetricalAlgorithm;

namespace DES
{
    public class DES : ISymmetricalAlgorithm
    {
        private const int BlockSizeConst = 8;

        private readonly ISymmetricalAlgorithm _feistelNetwork =
            new FeistelNetwork(new DESRoundKeyGenerator(), new DESFeistelFunction(), BlockSizeConst, 16);

        public int BlockSize => BlockSizeConst;

        public byte[] Encrypt(byte[] block, byte[][] roundKeys)
        {
            if (block.Length != BlockSizeConst)
            {
                throw new ArgumentException($"Block length must be equal to {BlockSizeConst}.");
            }

            block = Permutation(block, Tables.InitialPermutation);
            block = _feistelNetwork.Encrypt(block, roundKeys);

            return Permutation(block, Tables.FinalPermutation);
        }

        public byte[] Decrypt(byte[] block, byte[][] roundKeys)
        {
            if (block.Length != BlockSizeConst)
            {
                throw new ArgumentException($"Block length must be equal to {BlockSizeConst}.");
            }

            block = Permutation(block, Tables.InitialPermutation);
            block = _feistelNetwork.Decrypt(block, roundKeys);

            return Permutation(block, Tables.FinalPermutation);
        }

        public byte[][] GenerateRoundKeys(byte[] key)
        {
            return _feistelNetwork.GenerateRoundKeys(key);
        }

        public byte[] GenerateIV()
        {
            return GenerateRandomByteArray(BlockSizeConst);
        }
    }
}