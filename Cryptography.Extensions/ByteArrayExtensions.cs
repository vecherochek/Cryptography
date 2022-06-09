using System;

namespace Cryptography.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Xor(this byte[] left, byte[] right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }
            
            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }
            
            if (left.Length != right.Length)
            {
                throw new ArgumentException("Arrays lengths must be equal.");
            }

            for (var i = 0; i < left.Length; i++)
            {
                left[i] ^= right[i];
            }

            return left;
        }
        
    }
    
}