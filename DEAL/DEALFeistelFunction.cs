using CipherContext;
using SymmetricalAlgorithm;

namespace DEAL
{
    public class DEALFeistelFunction : IEncryptionTransformation
    {
        private readonly DES.DES _des;
        private readonly byte[] _iv;

        public DEALFeistelFunction(DES.DES des, byte[] iv)
        {
            _des = des;
            _iv = iv;
        }

        public byte[] Transform(byte[] block, byte[] roundKey)
        {
            var encoder = new CipherContext.CipherContext(_des, roundKey, _iv)
            {
                EncryptionMode = CipherContext.CipherContext.EncryptionModeList.CBC
            };

            return encoder.Encrypt(block, encoder.GenerateRoundKeys());
        }
    }
}