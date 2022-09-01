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
        public ISymmetricalAlgorithm Encoder { get; set; }

        public EncryptionModeList EncryptionMode
        {
            set
            {
                _encryptionMode = value switch
                {
                    EncryptionModeList.ECB => new ECB(Encoder),
                    EncryptionModeList.CBC => new CBC(Encoder),
                    EncryptionModeList.CFB => new CFB(Encoder),
                    EncryptionModeList.OFB => new OFB(Encoder),
                    EncryptionModeList.CTR => new CTR(Encoder),
                    EncryptionModeList.RD => new RD(Encoder),
                    EncryptionModeList.RDH => new RDH(Encoder),
                    _ => throw new ArgumentOutOfRangeException(nameof(_encryptionMode), _encryptionMode,
                        "No such encryption mode :(")
                };
            }
        }
        
        public CipherContext(byte[] key, params object[] values)
        {
            _key = key;
            _values = values;
        }

        public async Task<byte[]> EncryptAsync(byte[] message, byte[][] roundKeys)
        {
            //Console.WriteLine($"я начал работу\n");
            var original = PaddingPKCs7(message, Encoder.BlockSize);
            return await _encryptionMode.EncryptBlockAsync(original, roundKeys, _values).ConfigureAwait(false);
        }

        public async Task<byte[]> DecryptAsync(byte[] message, byte[][] roundKeys)
        {
            return await _encryptionMode.DecryptBlockAsync(message, roundKeys, _values).ConfigureAwait(false);
        }

        public byte[][] GenerateRoundKeys()
        {
            return Encoder.GenerateRoundKeys(_key);
        }
    }
}