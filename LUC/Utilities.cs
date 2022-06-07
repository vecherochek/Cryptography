using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace LUC
{
    public static class Utilities
    {
        //для генерации ключей
        public static BigInteger Legendre(BigInteger a, BigInteger p)
        {
            if (p < (BigInteger)2)
                throw new ArgumentOutOfRangeException(nameof(p), "P must be >= 2");
            
            if (a == BigInteger.Zero || a == BigInteger.One)
                return a;
            
            BigInteger result;
            if (BigInteger.Remainder(a, (BigInteger)2) == BigInteger.Zero)
            {
                result = Legendre(a / (BigInteger)2, p);
                if (((p * p - BigInteger.One) & (BigInteger)8) != BigInteger.Zero)
                    result = BigInteger.Negate(result);
            }
            else
            {
                result = Legendre(p % a, a);
                if (((a -  BigInteger.One) * (p - (BigInteger)1) & (BigInteger)4) != BigInteger.Zero)
                    result = BigInteger.Negate(result);
            }
            
            return result;
        }
        
        public static BigInteger Lcm(BigInteger a, BigInteger b) 
            => (a * b) / BigInteger.GreatestCommonDivisor(a, b);
        
        public static BigInteger MultiplicativeInverseModulo(BigInteger a, BigInteger m)
        {
            BigInteger g = Gcd(a, m, out BigInteger x, out BigInteger y);
            if (g != BigInteger.One) return BigInteger.Zero;  
            
            return (x % m + m) % m;;
        }
        
        private static BigInteger Gcd(BigInteger a, BigInteger m, out BigInteger x, out BigInteger y)
        {
            if (a == BigInteger.Zero)
            {
                x = BigInteger.Zero;
                y = BigInteger.One;
                return m;
            }

            BigInteger d = Gcd(m % a, a, out BigInteger x1, out BigInteger y1);
            
            x = y1 - (m / a) * x1;
            y = x1;
            
            return d;
        }
        //для тестов простоты
        public static BigInteger GenerateRandomBigInteger(BigInteger left, BigInteger right)
        {
            var random = RandomNumberGenerator.Create();
            var buffer = new byte[right.GetByteCount()];
            BigInteger randomNumber;
            do
            {
                random.GetBytes(buffer);
                randomNumber = new BigInteger(buffer);
            } while ((left > randomNumber) || (randomNumber > right));

            return randomNumber;
        }

        public static int Jacobi(BigInteger a, BigInteger b)
        {
            if (BigInteger.GreatestCommonDivisor(a, b) != 1) return 0;
            int r = 1;
            if (a < BigInteger.Zero)
            {
                a = BigInteger.Negate(a);
                if (BigInteger.Remainder(b, (BigInteger) 4) == 3)
                {
                    r = -r;
                }
            }

            do
            {
                int t = 0;
                while (BigInteger.Remainder(a, (BigInteger) 2) == 0)
                {
                    t++;
                    a /= (BigInteger) 2;
                }

                if (t % 2 != 0)
                {
                    if (BigInteger.Remainder(b, (BigInteger) 8) == 3 || BigInteger.Remainder(b, (BigInteger) 8) == 5)
                        r = -r;
                }

                if (BigInteger.Remainder(a, (BigInteger) 4) == 3 && BigInteger.Remainder(b, (BigInteger) 4) == 3)
                    r = -r;
                
                BigInteger c = a;
                a = BigInteger.Remainder(b, c);
                b = c;

            } while (a != (BigInteger) 0);

            return r;
        }
        //последовательность люка
        public static BigInteger LucasSequencesMod(BigInteger P, BigInteger n, BigInteger mod)
        { 
            BigInteger prev = P;
            BigInteger current = (P * P - (BigInteger)2) % n;
            var size = n.GetBitLength() - 2;
            
            var nInBinarySystem = string.Concat(n.ToByteArray().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse());
            for (var i = size; i > 0; i--)
            {
                if (nInBinarySystem[(int)(nInBinarySystem.Length - i)] == 1)
                {
                    prev = (prev * current - P) % mod;
                    current = (current * current - (BigInteger)2) % mod;
                }
                else if (nInBinarySystem[(int)(nInBinarySystem.Length - i)] == 0)
                {
                    current = (prev * current - P) % mod;
                    prev = (prev * prev - (BigInteger) 2) % mod;
                }
            }
            
            return prev;
        }
    }
}