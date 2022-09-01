using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SymmetricalAlgorithm;

namespace CipherContext.EncryptionModes
{
    internal class ECB : IEncryptionMode
    {
        private readonly ISymmetricalAlgorithm _encoder;

        public ECB(ISymmetricalAlgorithm encryptor)
        {
            _encoder = encryptor;
        }

        public byte[] EncryptBlock(byte[] message, byte[][] roundKeys, params object[] values)
        {
            var tasks = new List<Task<byte[]>>();
            for (var i = 0; i < message.Length / _encoder.BlockSize; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() =>
                {
                    var currentBlock = message.Skip(i1 * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();
                    return _encoder.Encrypt(currentBlock, roundKeys);
                }, default));
            }

            var result = Task.WhenAll(tasks).ConfigureAwait(false);
            return result.GetAwaiter().GetResult().SelectMany(block => block).ToArray();
        }

        public byte[] DecryptBlock(byte[] message, byte[][] roundKeys, params object[] values)
        {
            var tasks = new List<Task<byte[]>>();
            for (var i = 0; i < message.Length / _encoder.BlockSize; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() =>
                {
                    var currentBlock = message.Skip(i1 * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();
                    return _encoder.Decrypt(currentBlock, roundKeys);
                }, default));
            }

            var decrypted = Task.WhenAll(tasks).ConfigureAwait(false);
            var result = decrypted.GetAwaiter().GetResult().SelectMany(block => block).ToArray();
            Array.Resize(ref result, message.Length - result[^1]);
            return result;
        }
    }
}