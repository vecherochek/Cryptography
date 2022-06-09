using System.Numerics;

namespace LUC
{
    public struct PrimeNumbers
    {
        public BigInteger P { get;}

        public BigInteger Q { get;}
        
        public BigInteger N { get;}
        
        public PrimeNumbers(BigInteger p, BigInteger q)
        {
            P = p;
            Q = q;
            N = p * q;
        }

    }
}