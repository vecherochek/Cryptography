using System;
using System.Linq;
using System.Threading.Tasks;
using SymmetricalAlgorithm;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace CipherContext.EncryptionModes
{
    internal class OFB : IEncryptionMode
    {
        private readonly ISymmetricalAlgorithm _encoder;

        public OFB(ISymmetricalAlgorithm encryptor)
        {
            _encoder = encryptor;
        }

        private byte[] EncryptBlock(byte[] message, byte[][] roundKeys, object[] initializationVector)
        {
            var result = new byte[message.Length];
            var prevBlock = new byte[_encoder.BlockSize];

            Array.Copy((byte[]) initializationVector[0], prevBlock, prevBlock.Length);
            for (var i = 0; i < result.Length / _encoder.BlockSize; i++)
            {
                var currentBlock = message.Skip(i * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();

                prevBlock = _encoder.Encrypt(prevBlock, roundKeys);
                currentBlock.Xor(prevBlock);
                Array.Copy(currentBlock, 0, result, i * _encoder.BlockSize, _encoder.BlockSize);
            }

            return result;
        }

        private byte[] DecryptBlock(byte[] message, byte[][] roundKeys, object[] initializationVector)
        {
            var result = EncryptBlock(message, roundKeys, initializationVector);
            Array.Resize(ref result, message.Length - result[^1]);
            return result;
        }

        public async Task<byte[]> EncryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values)
        {
            return await Task.Run(() => EncryptBlock(message, roundKeys, values));
        }

        public async Task<byte[]> DecryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values)
        {
            return await Task.Run(() => DecryptBlock(message, roundKeys, values));
        }
    }
}