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
            var encoder = new CipherContext.CipherContext(roundKey, _iv)
            {
                Encoder = _des,
                EncryptionMode = EncryptionModeList.CBC
            };
            //encoder.EncryptAsync(block, encoder.GenerateRoundKeys()).RunSynchronously();
            
            //var task = Task.Run(() => encoder.EncryptAsync(block, encoder.GenerateRoundKeys())).ConfigureAwait(false);
            //task.RunSynchronously();
            //return task.GetAwaiter().GetResult();
            return encoder.EncryptAsync(block, encoder.GenerateRoundKeys()).ConfigureAwait(false).GetAwaiter().GetResult();
            //чо блин не так
        }
    }
}