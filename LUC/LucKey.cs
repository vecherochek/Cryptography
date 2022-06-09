using System.Numerics;

namespace LUC
{
    public struct LucKey
    {
        public BigInteger Key { get;}
            
        public BigInteger N { get;}
            
        public LucKey(BigInteger key, BigInteger n)
        {
            Key = key;
            N = n;
        }
    }
}