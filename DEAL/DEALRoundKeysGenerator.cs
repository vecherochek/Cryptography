using System;
using System.Linq;
using CipherContext;
using Cryptography.Extensions;
using SymmetricalAlgorithm;

namespace DEAL
{
    public class DEALRoundKeysGenerator : IRoundKeyGenerator
    {
        private readonly DES.DES _des;
        private const long ConstKey = 0x1234567890abcdef;
        private readonly byte[][] _bit64OriginalString;
        private byte[][] _roundKeys;
        private readonly byte[] _iv;

        public DEALRoundKeysGenerator(DES.DES des, byte[] iv)
        {
            _bit64OriginalString = new ulong[]
            {
                0x8000000000000000,
                0x0800000000000000,
                0x0008000000000000,
                0x0000000800000000
            }.Select(BitConverter.GetBytes).ToArray();
            _des = des;
            _iv = iv;
        }

        public byte[][] GenerateRoundKeys(byte[] key)
        {
            var encoder = new CipherContext.CipherContext(BitConverter.GetBytes(ConstKey), _iv)
            {
                Encoder = _des,
                EncryptionMode = EncryptionModeList.CBC
            };
            var desRoundKeys = encoder.GenerateRoundKeys();
            switch (key.Length)
            {
                case 16:
                {
                    _roundKeys = new byte[6][];

                    var k = new[]
                    {
                        key.Take(key.Length / 2).ToArray(),
                        key.Skip(key.Length / 2).ToArray()
                    };

                    _roundKeys[0] = encoder.EncryptAsync(k[0], desRoundKeys).GetAwaiter().GetResult();
                    _roundKeys[1] = encoder.EncryptAsync(k[1].Xor(_roundKeys[0]), desRoundKeys)
                        .GetAwaiter().GetResult();

                    for (var i = 0; i < 4; i++)
                    {
                        _roundKeys[i + 2] = encoder.EncryptAsync(k[i % 2]
                            .Xor(_bit64OriginalString[i])
                            .Xor(_roundKeys[i + 1]), desRoundKeys).GetAwaiter().GetResult();
                    }

                    break;
                }

                case 24:
                {
                    _roundKeys = new byte[6][];

                    var k = new[]
                    {
                        key.Take(key.Length / 3).ToArray(),
                        key.Skip(key.Length / 3).Take(key.Length / 3).ToArray(),
                        key.Skip(key.Length / 3 * 2).ToArray()
                    };

                    _roundKeys[0] = encoder.EncryptAsync(k[0], desRoundKeys).GetAwaiter().GetResult();

                    for (var i = 0; i < 2; i++)
                    {
                        _roundKeys[i + 1] = encoder.EncryptAsync(k[i % 3].Xor(_roundKeys[i]), desRoundKeys)
                            .GetAwaiter().GetResult();
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        _roundKeys[i + 3] = encoder.EncryptAsync(k[i % 3]
                            .Xor(_bit64OriginalString[i])
                            .Xor(_roundKeys[i + 1]), desRoundKeys).GetAwaiter().GetResult();
                    }

                    break;
                }
                case 32:
                {
                    _roundKeys = new byte[8][];

                    var k = new[]
                    {
                        key.Take(key.Length / 4).ToArray(),
                        key.Skip(key.Length / 4).Take(key.Length / 4).ToArray(),
                        key.Skip(key.Length / 4 * 2).Take(key.Length / 4).ToArray(),
                        key.Skip(key.Length / 4 * 3).ToArray()
                    };

                    _roundKeys[0] = encoder.EncryptAsync(k[0], desRoundKeys).GetAwaiter().GetResult();

                    for (var i = 0; i < 3; i++)
                    {
                        _roundKeys[i + 1] = encoder.EncryptAsync(k[i % 4].Xor(_roundKeys[i]), desRoundKeys)
                            .GetAwaiter().GetResult();
                    }

                    for (var i = 0; i < 4; i++)
                    {
                        _roundKeys[i + 4] = encoder.EncryptAsync(k[i % 4]
                            .Xor(_bit64OriginalString[i])
                            .Xor(_roundKeys[i + 1]), desRoundKeys).GetAwaiter().GetResult();
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(key), key.Length, "Invalid key");
            }

            return _roundKeys;
        }
    }
}