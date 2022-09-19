using System;
using System.Linq;
using DES;
using static Cryptography.Extensions.ByteArrayExtensions;
using SymmetricalAlgorithm;

namespace DEAL
{
    public class DEAL : ISymmetricalAlgorithm
    {
        private static readonly DES.DES Des = new DES.DES();
        private const int BlockSizeConst = 16;
        private readonly ISymmetricalAlgorithm _feistelNetwork;
        private byte[] _desIv;

        public int BlockSize => BlockSizeConst;

        public DEAL(byte[] key, byte[] desIv)
        {
            var numberOfRounds = key.Length == 32 ? 8 : 6;
            _desIv = desIv;
            _feistelNetwork =
                new FeistelNetwork(
                    new DEALRoundKeysGenerator(Des, _desIv),
                    new DEALFeistelFunction(Des, _desIv),
                    BlockSizeConst,
                    numberOfRounds);
        }

        public byte[] Encrypt(byte[] block, byte[][] roundKeys)
        {
            if (block.Length != BlockSizeConst)
            {
                throw new ArgumentException($"Block length must be equal to {BlockSizeConst}.");
            }
            
            var result = new byte[BlockSizeConst];

            for (var i = 0; i < result.Length / BlockSizeConst; i++)
            {
                var currentBlock = new byte[BlockSizeConst];
                Array.Copy(block, i * BlockSizeConst, currentBlock, 0, BlockSizeConst);
                var (left, right) = (currentBlock.Take(BlockSizeConst / 2).ToArray(),
                    currentBlock.Skip(BlockSizeConst / 2).ToArray());

                currentBlock = _feistelNetwork.Encrypt(right.Concat(left).ToArray(), roundKeys);

                (left, right) = (currentBlock.Take(BlockSizeConst / 2).ToArray(),
                    currentBlock.Skip(BlockSizeConst / 2).ToArray());
                Array.Copy(right.Concat(left).ToArray(), 0, result, i * BlockSizeConst, BlockSizeConst);
            }

            return result;
        }

        public byte[] Decrypt(byte[] block, byte[][] roundKeys)
        {
            if (block.Length != BlockSizeConst)
            {
                throw new ArgumentException($"Block length must be equal to {BlockSizeConst}.");
            }
            
            var result = new byte[BlockSizeConst];

            for (var i = 0; i < result.Length / BlockSizeConst; i++)
            {
                var currentBlock = new byte[BlockSizeConst];
                Array.Copy(block, i * BlockSizeConst, currentBlock, 0, BlockSizeConst);
                var (left, right) = (currentBlock.Take(BlockSizeConst / 2).ToArray(),
                    currentBlock.Skip(BlockSizeConst / 2).ToArray());

                currentBlock = _feistelNetwork.Decrypt(right.Concat(left).ToArray(), roundKeys);

                (left, right) = (currentBlock.Take(BlockSizeConst / 2).ToArray(),
                    currentBlock.Skip(BlockSizeConst / 2).ToArray());
                Array.Copy(right.Concat(left).ToArray(), 0, result, i * BlockSizeConst, BlockSizeConst);
            }

            return result;
        }

        public byte[][] GenerateRoundKeys(byte[] key)
        {
            return _feistelNetwork.GenerateRoundKeys(key);
        }

        public byte[] GenerateIV()
        {
            return GenerateRandomByteArray(BlockSize);
        }
    }
}