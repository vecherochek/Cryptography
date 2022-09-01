using System.Threading.Tasks;

namespace CipherContext.EncryptionModes
{
    public interface IEncryptionMode
    {
        public byte[] EncryptBlock(byte[] message, byte[][] roundKeys, params object[] values);
        public byte[] DecryptBlock(byte[] message, byte[][] roundKeys, params object[] values);
    }
}