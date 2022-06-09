using System.Numerics;
using System.Security.Cryptography;

namespace LUC
{
    public class PrimeNumbersGenerator
    {
        private readonly long _length;
        private readonly PrimeNumberTest _test;
        private readonly double _probability;
        
        public PrimeNumbersGenerator(long length, PrimeNumberTest test, double probability)
        {
            _length = length;
            _test = test;
            _probability = probability;
        }
        public PrimeNumbers GeneratePrime()
        {
            BigInteger N, p, q;
            do
            {
                p = RandomPrimeNumberGenerator();
                q = RandomPrimeNumberGenerator();
                N = p * q;

            } while (BigInteger.Compare(N.GetBitLength(), _length) < 0);

            return new PrimeNumbers(p, q);
        }
        private BigInteger RandomPrimeNumberGenerator()
        {
            var random = RandomNumberGenerator.Create();
            var buffer = new byte[_length / 2 + 1];
            while (true)
            {
                random.GetBytes(buffer);
                buffer[0] |= 0x01;
                var primeNumber = new BigInteger(buffer);
                
                if ((BigInteger)2 > primeNumber) continue;

                switch (_test)
                {
                    case PrimeNumberTest.Fermat:
                        if (PrimalityTests.FermatPrimalityTest(primeNumber, _probability))
                            return primeNumber;
                        break;
                    case PrimeNumberTest.SolovayStrassen:
                        if (PrimalityTests.SolovayStrassenPrimalityTest(primeNumber, _probability))
                            return primeNumber;
                        break;
                    case PrimeNumberTest.MillerRabin:
                        if (PrimalityTests.MillerRabinPrimalityTest(primeNumber, _probability))
                            return primeNumber;
                        break;
                }
            }
        }
    }
}