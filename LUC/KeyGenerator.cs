using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using static Cryptography.Extensions.BigIntegerExtensions;

namespace LUC
{
    public class KeyGenerator
    {
        private PrimeNumbers _primeNumbers;
        private readonly BigInteger _message;
        public LucKey PublicKey { get; set; }

        public LucKey PrivateKey { get; set; }

        public KeyGenerator(BigInteger message)
        {
            _message = message;
        }

        public void GenerateKey()
        {
            var prime = new PrimeNumbersGenerator(_message.GetByteCount(), PrimeNumberTest.MillerRabin, 0.75);
            _primeNumbers = prime.GeneratePrime();
            
            BigInteger e = GetE();
            BigInteger D = _message * _message - 4;
            BigInteger S = Lcm(_primeNumbers.P - Legendre(D, _primeNumbers.P), _primeNumbers.Q - Legendre(D, _primeNumbers.Q));
            BigInteger d = MultiplicativeInverseModulo(e, S);
            
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
                e = GenerateRandomBigInteger(2, _primeNumbers.N);
            } while (BigInteger.GreatestCommonDivisor(e, number) != 1);

            return e;
        }

        public Task GenerateKeyAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            return Task.Run(() => GenerateKey(), token);
        }
    }
}