using System;
using System.Linq;

using Cryptography.Extensions;

namespace DEAL
{
    public class KeysGenerator
    {
        private readonly long _const_key = 0x1234567890abcdef;

        private readonly byte[][] _bit64OriginalString;
        private readonly byte[] _key;
        private byte[][] _roundKeys;

        public KeysGenerator(byte[] key)
        {
            _bit64OriginalString = new ulong[]
            {
                0x8000000000000000,
                0x0800000000000000,
                0x0008000000000000,
                0x0000000800000000
            }.Select(BitConverter.GetBytes).ToArray();
            
            _key = key;
        }
        
        public byte[][] GenerateRoundKeys()
        {
            var des = new DES.DES(BitConverter.GetBytes(_const_key), DES.EncryptionModes.CBC, new byte[]{1,1,1,1,1,1,1,1});
            
            switch (_key.Length)
            {
                case 16:
                {
                    _roundKeys = new byte[6][];

                    var k = new []
                    {
                        _key.Take(_key.Length / 2).ToArray(),
                        _key.Skip(_key.Length / 2).ToArray()
                    };
                    
                    _roundKeys[0] = des.EncryptBlock(k[0]);
                    _roundKeys[1] = des.EncryptBlock(k[1].Xor(_roundKeys[0]));

                    for (var i = 0; i < 4; i++)
                    {
                        _roundKeys[i + 2] = des.EncryptBlock(k[i % 2]
                            .Xor(_bit64OriginalString[i])
                            .Xor(_roundKeys[i + 1]));
                    }
                    
                    break;
                }

                case 24:
                {
                    var k = new []
                    {
                        _key.Take(_key.Length / 3).ToArray(),
                        _key.Skip(_key.Length / 3).Take(_key.Length / 3).ToArray(),
                        _key.Skip(_key.Length / 3 * 2).ToArray()
                    };
                    
                    _roundKeys[0] = des.EncryptBlock(k[0]);
                    
                    for (var i = 0; i < 2; i++)
                    {
                        _roundKeys[i + 1] = des.EncryptBlock(k[i % 3].Xor(_roundKeys[i]));
                    }
                    
                    for (var i = 0; i < 3; i++)
                    {
                        _roundKeys[i + 3] = des.EncryptBlock(k[i % 3]
                            .Xor(_bit64OriginalString[i])
                            .Xor(_roundKeys[i + 1]));
                    }
       
                    break;
                }
                case 32:
                {
                    var k = new []
                    {
                        _key.Take(_key.Length / 4).ToArray(),
                        _key.Skip(_key.Length / 4).Take(_key.Length / 4).ToArray(),
                        _key.Skip(_key.Length / 4 * 2).Take(_key.Length / 4).ToArray(),
                        _key.Skip(_key.Length / 4 * 3).ToArray()
                    };
                    
                    _roundKeys[0] = des.EncryptBlock(k[0]);
                    
                    for (var i = 0; i < 3; i++)
                    {
                        _roundKeys[i + 1] = des.EncryptBlock(k[i % 4].Xor(_roundKeys[i]));
                    }
                    
                    for (var i = 0; i < 4; i++)
                    {
                        _roundKeys[i + 4] = des.EncryptBlock(k[i % 4]
                            .Xor(_bit64OriginalString[i])
                            .Xor(_roundKeys[i + 1]));
                    }
                    
                    break;
                }
            }
            
            return _roundKeys;
        }
    }
}