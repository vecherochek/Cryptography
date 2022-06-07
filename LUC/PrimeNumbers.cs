using System.Numerics;

namespace LUC
{
    public struct PrimeNumbers
    {
        public BigInteger P { get; private set; }

        public BigInteger Q { get; private set; }
        
        public BigInteger N { get; private set; }
        
        public PrimeNumbers(BigInteger p, BigInteger q)
        {
            P = p;
            Q = q;
            N = p * q;
        }

    }
}