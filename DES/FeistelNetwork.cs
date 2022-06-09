using System;
using Cryptography.Extensions;

namespace DES
{
    public class FeistelNetwork
    {
        private readonly ulong[] _roundKeys;
        
        public FeistelNetwork(ulong[] roundKeys)
        {
            _roundKeys = roundKeys;
        }
        
        public byte[] Encrypt (byte[] block) 
        {
            var number = BitConverter.ToUInt64(block, 0);

            var left = number >> 32;
            var right = (number & ((ulong)1 << 32) - 1);
            
            for (var i = 0; i < 16; i++)
            {
                var tmp = right;
                right = left ^ FeistelFunction(right, _roundKeys[i]);
                left = tmp;
            }

            return BitConverter.GetBytes((left << 32 | right));
        }
        
        public byte[] Decrypt (byte[] block) 
        {
            var number = BitConverter.ToUInt64(block, 0);

            var left = number >> 32;
            var right = (number & ((ulong)1 << 32) - 1);

            for (var i = 15; i >= 0; i--)
            {
                var tmp = left;
                left = right ^ FeistelFunction(left, _roundKeys[i]);
                right = tmp;
            }

            return BitConverter.GetBytes((left << 32 | right));
        }
        
        private static ulong FeistelFunction(ulong block, ulong roundKey)
        {
            var value = BitConverter.GetBytes(block);
            var expandingPermutation = BitConverter.ToUInt32(ByteArrayExtensions.Permutation32(value, Tables.ExpandingPermutation), 0);
            var xor = expandingPermutation ^ roundKey;

            ulong result = 0;
            for (var i = 0; i < 8; i++)
            {
                var B = (xor >> (i * 6)) & ((uint)1 << 6) - 1;
                var a = ((B >> 5) << 1) | (B & 1);
                var b = (B >> 1) & 0b1111;

                B = Tables.SBlocks[i, a, b];
                result|= B << i * 4;
            }
            value = BitConverter.GetBytes(result);
            
            return BitConverter.ToUInt32(ByteArrayExtensions.Permutation32(value, Tables.PBlock), 0);
        }
    }
}