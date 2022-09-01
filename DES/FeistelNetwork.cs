using System;
using System.Linq;
using Cryptography.Extensions;
using SymmetricalAlgorithm;

namespace DES
{
    public class FeistelNetwork : ISymmetricalAlgorithm
    {
        private readonly IRoundKeyGenerator _roundKeysGenerator;
        private readonly IEncryptionTransformation _feistelFunction;
        private readonly int _blockSize;
        private readonly int _numberOfRounds;
        public FeistelNetwork(IRoundKeyGenerator roundKeysGenerator, IEncryptionTransformation feistelFunction,
            int blockSize, int numberOfRounds)
        {
            _roundKeysGenerator = roundKeysGenerator;
            _feistelFunction = feistelFunction;
            _blockSize = blockSize;
            _numberOfRounds = numberOfRounds;
        }

        public int BlockSize => _blockSize;
        

        public byte[] Encrypt(byte[] block, byte[][] roundKeys)
        {
            if (block.Length != _blockSize)
            {
                throw new ArgumentException($"Block length must be equal to {_blockSize}.");
            }

            var left = block.Take(_blockSize / 2).ToArray();
            var right = block.Skip(_blockSize / 2).ToArray();
            for (var i = 0; i < _numberOfRounds; i++)
            {
                var tmp = right;
                right = left.Xor(_feistelFunction.Transform(right, roundKeys[i]));
                left = tmp;
            }

            return left.Concat(right).ToArray();
        }

        public byte[] Decrypt(byte[] block, byte[][] roundKeys)
        {
            if (block.Length != _blockSize)
            {
                throw new ArgumentException($"Block length must be equal to {_blockSize}.");
            }

            var left = block.Take(_blockSize / 2).ToArray();
            var right = block.Skip(_blockSize / 2).ToArray();
            for (var i = _numberOfRounds - 1; i >= 0; i--)
            {
                var tmp = left;
                left = right.Xor(_feistelFunction.Transform(left, roundKeys[i]));
                right = tmp;
            }

            return left.Concat(right).ToArray();
        }

        public byte[][] GenerateRoundKeys(byte[] key)
        {
            return _roundKeysGenerator.GenerateRoundKeys(key);
        }
    }
}