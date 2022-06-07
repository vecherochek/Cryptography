using System;
using System.Numerics;
using static LUC.Utilities;

namespace LUC
{
    public static class PrimalityTests
    {
        public static bool FermatPrimalityTest(BigInteger number, double probability)
        {
            if (number == (BigInteger)1) return false;
            for (var k = 1; 1 - Math.Pow(2, -k) <= probability; k++)
            {
                BigInteger a = GenerateRandomBigInteger((BigInteger) 1, number);
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
                BigInteger a = GenerateRandomBigInteger((BigInteger) 1, number);
                if (BigInteger.GreatestCommonDivisor(a, number) > BigInteger.One) 
                    return false;
                if (BigInteger.ModPow(a, (number - BigInteger.One) / (BigInteger) 2, number) != (BigInteger) Jacobi(a, number))
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
            while (BigInteger.GreatestCommonDivisor(a, (BigInteger) 2) == 0)
            {
                a /= (BigInteger) 2;
                s++;
            }

            for (int k = 1; 1 - Math.Pow(4, -k) <= probability; k++)
            {
                var b = GenerateRandomBigInteger((BigInteger) 1, a);
                var x = BigInteger.ModPow(b, a, number);
                
                if (BigInteger.Compare(x, (BigInteger)1) == 0 || BigInteger.Compare(x, (BigInteger)a) == 0)
                    continue;
                
                for (var i = 0; i < s - 1; i++)
                {
                    x = BigInteger.ModPow(x, (BigInteger)2, number);
                    if (x == BigInteger.One) 
                        return false;
                    if (x == (BigInteger)a)
                        break;
                }
                if (x != (BigInteger)a)
                    return false;
            }
            
            return true;
        }
    }
}