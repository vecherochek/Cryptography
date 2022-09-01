using System;
using System.Numerics;
using static Cryptography.Extensions.BigIntegerExtensions;

namespace LUC
{
    public static class PrimalityTests
    {
        public static bool FermatPrimalityTest(BigInteger number, double probability)
        {
            if (number == (BigInteger)1) return false;
            for (var k = 1; 1 - Math.Pow(2, -k) <= probability; k++)
            {
                BigInteger a = GenerateRandomBigInteger(1, number);
                if (BigInteger.ModPow(a, number - BigInteger.One, number) != 1)
                    return false;
            }

            return true;
        }
        public static bool SolovayStrassenPrimalityTest(BigInteger number, double probability)
        {
            if (number == BigInteger.One) return false;
            for (var k = 1; 1 - Math.Pow(2, -k) <= probability; k++)
            {
                BigInteger a = GenerateRandomBigInteger(1, number);
                if (BigInteger.GreatestCommonDivisor(a, number) > BigInteger.One) 
                    return false;
                if (BigInteger.ModPow(a, (number - BigInteger.One) / 2, number) != (BigInteger) Jacobi(a, number))
                    return false;
            }

            return true;
        }
        public static bool MillerRabinPrimalityTest(BigInteger number, double probability)
        {
            if (number == (BigInteger)3) return true;
            if (number == (BigInteger)2) return true;
            if (number < (BigInteger)2) return false;

            var a = number - BigInteger.One;
            int s = 0;
            while (BigInteger.GreatestCommonDivisor(a, 2) == 0)
            {
                a /= 2;
                s++;
            }

            for (int k = 1; 1 - Math.Pow(4, -k) <= probability; k++)
            {
                var b = GenerateRandomBigInteger(1, a);
                var x = BigInteger.ModPow(b, a, number);
                
                if (BigInteger.Compare(x, 1) == 0 || BigInteger.Compare(x, a) == 0)
                    continue;
                
                for (var i = 0; i < s - 1; i++)
                {
                    x = BigInteger.ModPow(x, 2, number);
                    if (x == BigInteger.One) 
                        return false;
                    if (x == a)
                        break;
                }
                if (x != a)
                    return false;
            }
            
            return true;
        }
    }
}