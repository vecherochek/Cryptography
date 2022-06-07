using System.Numerics;

namespace LUC
{
    public struct LucKey
    {
        public BigInteger Key { get; private set; }
            
        public BigInteger N { get; private set; }
            
        public LucKey(BigInteger key, BigInteger n)
        {
            Key = key;
            N = n;
        }
    }
}