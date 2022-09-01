using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SymmetricalAlgorithm;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace CipherContext.EncryptionModes
{
    internal class RDH : IEncryptionMode
    {
        private ISymmetricalAlgorithm _encoder;

        public RDH(ISymmetricalAlgorithm encryptor)
        {
            _encoder = encryptor;
        }

        public async Task<byte[]> EncryptBlockAsync(byte[] message, byte[][] roundKeys, object[] values)
        {
            var initial = new byte[_encoder.BlockSize];
            var hash = new byte[_encoder.BlockSize];

            Array.Copy((byte[]) values[0], initial, _encoder.BlockSize);
            Array.Copy(
                PaddingPKCs7(BitConverter.GetBytes(values[1].GetHashCode()), _encoder.BlockSize),
                hash, _encoder.BlockSize);

            var initialBlock = _encoder.Encrypt(initial, roundKeys)
                .Concat(_encoder.Encrypt(hash.Xor(initial), roundKeys));
            //для дельты всегда берем последние 8 байт вектора инициализации
            var delta = initial.Skip(initial.Length - 8).ToArray();
            //делаем дельту нечетной
            delta[0] |= 0x01;

            var tasks = new List<Task<byte[]>>();
            for (var i = 0; i < message.Length / _encoder.BlockSize; i++)
            {
                var i1 = i;
                initial = ByteArrayAdditionByModulo2PowN(initial, delta);
                var initial1 = initial;
                tasks.Add(Task.Run(() =>
                {
                    var currentBlock = message.Skip(i1 * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();
                    return _encoder.Encrypt(currentBlock.Xor(initial1), roundKeys);
                }, default));
            }

            var result = await Task.WhenAll(tasks);
            return initialBlock.Concat(result.SelectMany(block => block).ToArray()).ToArray();
        }

        public async Task<byte[]> DecryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values)
        {
            var initial = _encoder.Decrypt(message.Take(_encoder.BlockSize).ToArray(), roundKeys);
            var delta = initial.Skip(initial.Length - 8).ToArray();
            delta[0] |= 0x01;
            //var hash = _encoder.Decrypt(message.Skip(_encoder.BlockSize).Take(_encoder.BlockSize).ToArray(), roundKeys).Xor(initial);

            var tasks = new List<Task<byte[]>>();
            for (var i = 2; i < message.Length / _encoder.BlockSize; i++)
            {
                var i1 = i;
                initial = ByteArrayAdditionByModulo2PowN(initial, delta);
                var initial1 = initial;
                tasks.Add(Task.Run(() =>
                {
                    var currentBlock = message.Skip(i1 * _encoder.BlockSize).Take(_encoder.BlockSize).ToArray();
                    return _encoder.Decrypt(currentBlock, roundKeys).Xor(initial1);
                }, default));
            }

            var decrypted = await Task.WhenAll(tasks);
            var result = decrypted.SelectMany(block => block).ToArray();
            Array.Resize(ref result, result.Length - result[^1]);
            return result;
        }
    }
}