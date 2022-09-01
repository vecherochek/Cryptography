using System.Threading.Tasks;

namespace CipherContext.EncryptionModes
{
    public interface IEncryptionMode
    {
        public Task<byte[]> EncryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values);
        public Task<byte[]> DecryptBlockAsync(byte[] message, byte[][] roundKeys, params object[] values);
    }
}