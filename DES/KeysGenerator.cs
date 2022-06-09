using System;
using Cryptography.Extensions;

namespace DES
{
    public class KeysGenerator
    {
        private readonly byte[] _key;
        
        public KeysGenerator(byte[] key)
        {
            _key = key;
        }
        
        public ulong[] GenerateRoundKeys()
        {
            var firstPermutation = ByteArrayExtensions.Permutation64To56(_key, Tables.KeyPermutation);
            var currentC = firstPermutation >> 28;
            var currentD = firstPermutation & ((1 << 28) - 1);
            var roundKeys = new ulong[16];

            for (var i = 0; i < 16; ++i)
            {
                currentC = Shift(currentC, Tables.KeyShift[i]);
                currentD = Shift(currentD, Tables.KeyShift[i]);
                
                var currentKey = (currentC << 28) | currentD;
                roundKeys[i] =  BitConverter.ToUInt64(ByteArrayExtensions.Permutation64( BitConverter.GetBytes(currentKey), Tables.KeyСompressionPermutation), 0);
            }

            return roundKeys;
        }

        private ulong Shift(ulong number, byte shift)
            => ((number << shift) | (number >> (28 - shift)))& ((1 << 28) - 1);

    }
}