using System;
using System.Numerics;

namespace DEAL
{
    public class KeysGenerator
    {
        private readonly long _const_key = 0x1234567890abcdef;
        private readonly byte[] _key;
        private byte[][] _roundKeys;

        public KeysGenerator(byte[] key)
        {
            _key = key;
        }
        
        public byte[][] GenerateRoundKeys()
        {
            var des = new DES.DES(BitConverter.GetBytes(_const_key), DES.EncryptionModes.CBC, new byte[]{1,1,1,1,1,1,1,1});
            
            switch (_key.Length)
            {
                case 16:
                {
                    ulong k1 = BitConverter.ToUInt64(_key,0)>> 32;
                    ulong k2 = (BitConverter.ToUInt64(_key, 0) & ((ulong) 1 << 32) - 1);
                    _roundKeys = new byte[6][];
                    _roundKeys[0] = des.Encrypt(BitConverter.GetBytes(k1));
                    _roundKeys[1] = des.Encrypt(BitConverter.GetBytes(k2 ^ BitConverter.ToUInt64(_roundKeys[0], 0)));
                    _roundKeys[2] = des.Encrypt(BitConverter.GetBytes(k1 ^ 0x8000000000000000 ^ BitConverter.ToUInt64(_roundKeys[1], 0)));
                    _roundKeys[3] = des.Encrypt(BitConverter.GetBytes(k2 ^ 0x0800000000000000 ^ BitConverter.ToUInt64(_roundKeys[2], 0)));
                    _roundKeys[4] = des.Encrypt(BitConverter.GetBytes(k1 ^ 0x0008000000000000 ^ BitConverter.ToUInt64(_roundKeys[3], 0)));
                    _roundKeys[5] = des.Encrypt(BitConverter.GetBytes(k2 ^ 0x0000000800000000 ^ BitConverter.ToUInt64(_roundKeys[4], 0)));
                    break;
                }

                case 24:
                {
                    BigInteger k1 = new BigInteger(_key) >> 64;
                    BigInteger k2 = new BigInteger(_key) & (BigInteger.One << 64) - BigInteger.One;
                    BigInteger k3 = (new BigInteger(_key) >> 64) & ((BigInteger.One << 64) - BigInteger.One);
                    _roundKeys = new byte[6][];
                    _roundKeys[0] = des.Encrypt(k1.ToByteArray());
                    _roundKeys[1] = des.Encrypt((k2 ^ new BigInteger(_roundKeys[0])).ToByteArray());
                    _roundKeys[2] = des.Encrypt((k3 ^ new BigInteger(_roundKeys[1])).ToByteArray());
                    _roundKeys[3] = des.Encrypt((k1 ^ 0x8000000000000000 ^ new BigInteger(_roundKeys[2])).ToByteArray());
                    _roundKeys[4] = des.Encrypt((k2 ^ 0x0800000000000000 ^ new BigInteger(_roundKeys[3])).ToByteArray());
                    _roundKeys[5] = des.Encrypt((k3 ^ 0x0008000000000000 ^ new BigInteger(_roundKeys[4])).ToByteArray());
                    break;
                }
                case 32:
                {
                    BigInteger k1 = new BigInteger(_key) >> 64;
                    BigInteger k2 = new BigInteger(_key) & (BigInteger.One << 64) - BigInteger.One;
                    BigInteger k3 = (new BigInteger(_key) >> 64) & ((BigInteger.One << 128) - BigInteger.One);
                    BigInteger k4 = (new BigInteger(_key) >> 128) & ((BigInteger.One << 64) - BigInteger.One);
                    _roundKeys = new byte[8][];
                    _roundKeys[0] = des.Encrypt(k1.ToByteArray());
                    _roundKeys[1] = des.Encrypt((k2 ^ new BigInteger(_roundKeys[0])).ToByteArray());
                    _roundKeys[2] = des.Encrypt((k3 ^ new BigInteger(_roundKeys[1])).ToByteArray());
                    _roundKeys[3] = des.Encrypt((k4 ^ new BigInteger(_roundKeys[2])).ToByteArray());
                    _roundKeys[4] = des.Encrypt((k1 ^ 0x8000000000000000 ^ new BigInteger(_roundKeys[3])).ToByteArray());
                    _roundKeys[5] = des.Encrypt((k2 ^ 0x0800000000000000 ^ new BigInteger(_roundKeys[4])).ToByteArray());
                    _roundKeys[6] = des.Encrypt((k3 ^ 0x0008000000000000 ^ new BigInteger(_roundKeys[5])).ToByteArray());
                    _roundKeys[7] = des.Encrypt((k4 ^ 0x0000000800000000 ^ new BigInteger(_roundKeys[6])).ToByteArray());
                    break;
                }
            }
            return _roundKeys;
        }
    }
}