using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static Cryptography.Extensions.BigIntegerExtensions;

namespace LUC
{
    public class LUC
    {
        public BigInteger Encrypt(BigInteger message, LucKey publicKey) 
            => LucasSequencesMod(message, publicKey.Key, publicKey.N);
        
        public BigInteger Decrypt(BigInteger message, LucKey privateKey) 
            => LucasSequencesMod(message,  privateKey.Key,  privateKey.N);

        public Task<BigInteger> EncryptAsync(BigInteger message, LucKey publicKey, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.Run(() => Encrypt(message, publicKey), token);
        }
        public Task<BigInteger> DecryptAsync(BigInteger message, LucKey privateKe, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.Run(() => Decrypt(message, privateKe), token);
        }
    }
}