using System.Numerics;
using static LUC.Utilities;

namespace LUC
{
    public class LUC
    {
        public BigInteger Encrypt(BigInteger message, LucKey publicKey) 
            => LucasSequencesMod(message, publicKey.Key, publicKey.N);
        
        public BigInteger Decrypt(BigInteger message, LucKey privateKey) 
            => LucasSequencesMod(message,  privateKey.Key,  privateKey.N);
    }
}