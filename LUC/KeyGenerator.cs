using System.Numerics;
using Cryptography.Extensions;

namespace LUC
{
    public class KeyGenerator
    {
        private readonly PrimeNumbers _primeNumbers;
        public LucKey PublicKey { get; }

        public LucKey PrivateKey { get; }

        public KeyGenerator(BigInteger message)
        {
            var prime = new PrimeNumbersGenerator(message.GetByteCount(), PrimeNumberTest.MillerRabin, 0.75);
            _primeNumbers = prime.GeneratePrime();
            
            BigInteger e = GetE();
            BigInteger D = message * message - 4;
            BigInteger S = BigIntegerExtensions.Lcm(_primeNumbers.P - BigIntegerExtensions.Legendre(D, _primeNumbers.P), _primeNumbers.Q - BigIntegerExtensions.Legendre(D, _primeNumbers.Q));
            BigInteger d = BigIntegerExtensions.MultiplicativeInverseModulo(e, S);
            
            PublicKey = new LucKey(e, _primeNumbers.N);
            PrivateKey = new LucKey(d, _primeNumbers.N);
        }
        
        private BigInteger GetE()
        {
            BigInteger number = (_primeNumbers.P - 1) * (_primeNumbers.Q - 1) *
                              (_primeNumbers.P + 1) * (_primeNumbers.Q + 1);
            
            BigInteger e;
            do
            {
                e = BigIntegerExtensions.GenerateRandomBigInteger(2, _primeNumbers.N);
            } while (BigInteger.GreatestCommonDivisor(e, number) != 1);

            return e;
        }
    }
}