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

        public async Task<byte[]> EncryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values)
        {
            var tasks = new List<Task<byte[]>>();
            for (var i = 0; i < message.Length / _encoder.BlockSize; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() =>
                {
                    var currentBlock = message.Skip(i1 * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();
                    var a = _encoder.Encrypt(currentBlock, roundKeys);
                    Console.WriteLine(i1);
                    return a;
                }, default));
            }

            var result = await Task.WhenAll(tasks).ConfigureAwait(false);
            return result.SelectMany(block => block).ToArray();
        }

        public async Task<byte[]> DecryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values)
        {
            var tasks = new List<Task<byte[]>>();
            for (var i = 0; i < message.Length / _encoder.BlockSize; i++)
            {
                var i1 = i;
                tasks.Add(Task.Run(() =>
                {
                    var currentBlock = message.Skip(i1 * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();
                    Console.WriteLine(i1);
                    return _encoder.Decrypt(currentBlock, roundKeys);
                }, default));
            }

            var decrypted = await Task.WhenAll(tasks).ConfigureAwait(false);
            var result = decrypted.SelectMany(block => block).ToArray();
            Array.Resize(ref result, message.Length - result[^1]);
            return result;
        }
    }
}