using System.Numerics;
using static LUC.Utilities;

namespace LUC
{
    public class KeyGenerator
    {
        private readonly PrimeNumbers _primeNumbers;
        private LucKey _publicKey;
        private LucKey _privateKey;
        public LucKey PublicKey
        {
            get
            {
                return _publicKey;
            }
            private set
            {
                _publicKey = value;
            }
        }
        public LucKey PrivateKey
        {
            get
            {
                return _privateKey;
            }
            private set
            {
                _privateKey = value;
            }
        }
        public KeyGenerator(BigInteger message)
        {
            var prime = new PrimeNumbersGenerator(message.GetByteCount(), PrimeNumberTest.MillerRabin, 0.75);
            _primeNumbers = prime.GeneratePrime();
            
            BigInteger e = GetE();
            BigInteger D = message * message - (BigInteger) 4;
            BigInteger S = Lcm(_primeNumbers.P - Legendre(D, _primeNumbers.P), _primeNumbers.Q - Legendre(D, _primeNumbers.Q));
            BigInteger d = MultiplicativeInverseModulo(e, S);
            
            PublicKey = new LucKey(e, _primeNumbers.N);
            PrivateKey = new LucKey(d, _primeNumbers.N);
        }
        
        private BigInteger GetE()
        {
            BigInteger number = (_primeNumbers.P - (BigInteger) 1) * (_primeNumbers.Q - (BigInteger) 1) *
                              (_primeNumbers.P + (BigInteger) 1) * (_primeNumbers.Q + (BigInteger) 1);
            
            BigInteger e;
            do
            {
                e = GenerateRandomBigInteger((BigInteger) 2, _primeNumbers.N);
            } while (BigInteger.GreatestCommonDivisor(e, number) != 1);

            return e;
        }
    }
}