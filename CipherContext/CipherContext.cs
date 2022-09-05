using System;
using System.Threading.Tasks;
using CipherContext.EncryptionModes;
using SymmetricalAlgorithm;
using static Cryptography.Extensions.ByteArrayExtensions;

namespace CipherContext
{
    public enum EncryptionModeList
    {
        ECB,
        CBC,
        CFB,
        OFB,
        CTR,
        RD,
        RDH
    };

    public class CipherContext
    {
        private readonly byte[] _key;
        private readonly object[] _values;
        private IEncryptionMode _encryptionMode;
        private ISymmetricalAlgorithm _encoder;

        public EncryptionModeList EncryptionMode
        {
            set
            {
                _encryptionMode = value switch
                {
                    EncryptionModeList.ECB => new ECB(_encoder),
                    EncryptionModeList.CBC => new CBC(_encoder),
                    EncryptionModeList.CFB => new CFB(_encoder),
                    EncryptionModeList.OFB => new OFB(_encoder),
                    EncryptionModeList.CTR => new CTR(_encoder),
                    EncryptionModeList.RD => new RD(_encoder),
                    EncryptionModeList.RDH => new RDH(_encoder),
                    _ => throw new ArgumentOutOfRangeException(nameof(_encryptionMode), _encryptionMode,
                        "No such encryption mode :(")
                };
            }
        }

        public CipherContext(ISymmetricalAlgorithm encoder, byte[] key, params object[] values)
        {
            _encoder = encoder;
            _key = key;
            _values = values;
        }

        public Task<byte[]> EncryptAsync(byte[] message, byte[][] roundKeys)
        {
            /*await Task.Delay(3000);
            Console.WriteLine("ger-ger");*/
            return Task.Run(() => Encrypt(message, roundKeys), default);
        }

        public Task<byte[]> DecryptAsync(byte[] message, byte[][] roundKeys)
        {
            return Task.Run(() => Decrypt(message, roundKeys), default);
        }

        public byte[] Encrypt(byte[] message, byte[][] roundKeys)
        {
            var original = PaddingPKCs7(message, _encoder.BlockSize);
            return _encryptionMode.EncryptBlock(original, roundKeys, _values);
        }

        public byte[] Decrypt(byte[] message, byte[][] roundKeys)
        {
            return _encryptionMode.DecryptBlock(message, roundKeys, _values);
        }

        public byte[][] GenerateRoundKeys()
        {
            return _encoder.GenerateRoundKeys(_key);
        }
    }
}