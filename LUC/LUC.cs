using System.Numerics;
using Cryptography.Extensions;

namespace LUC
{
    public class LUC
    {
        public BigInteger Encrypt(BigInteger message, LucKey publicKey) 
            => BigIntegerExtensions.LucasSequencesMod(message, publicKey.Key, publicKey.N);
        
        public BigInteger Decrypt(BigInteger message, LucKey privateKey) 
            => BigIntegerExtensions.LucasSequencesMod(message,  privateKey.Key,  privateKey.N);
    }
}