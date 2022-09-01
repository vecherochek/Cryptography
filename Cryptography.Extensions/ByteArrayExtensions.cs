using System;
using System.Collections;
using System.Security.Cryptography;

namespace Cryptography.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Permutation(byte[] block, byte[] permutationTable)
        {
            var bitsblock = new BitArray(block);

            var changed = new BitArray(permutationTable.Length);
            for (var i = 0; i < permutationTable.Length; i++)
            {
                changed[i] = bitsblock[permutationTable[i] - 1];
            }

            return BitArrayToByteArray(changed);
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            var bytes = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static byte[] PermutationSBlock(byte[] block, byte[,,] permutationTable)
        {
            var number = BitConverter.ToUInt32(block, 0);
            ulong result = 0;
            for (var i = 0; i < 8; i++)
            {
                var B = (number >> (i * 6)) & ((uint) 1 << 6) - 1;
                var a = ((B >> 5) << 1) | (B & 1);
                var b = (B >> 1) & 0b1111;

                B = permutationTable[i, a, b];
                result |= B << i * 4;
            }

            return BitConverter.GetBytes(result);
        }

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
                left[i] = (byte) (left[i] ^ right[i]);
            }

            return left;
        }

        public static byte[] PaddingPKCs7(byte[] block, int blockSize)
        {
            if (block.Length == 8) return block;
            var addition = (byte) (blockSize - block.Length % blockSize);
            var paddedBlock = new byte[block.Length + addition];
            Array.Copy(block, paddedBlock, block.Length);
            Array.Fill(paddedBlock, addition, block.Length, addition);

            return paddedBlock;
        }

        public static byte[] ByteArrayAdditionByModulo2PowN(byte[] left, byte[] right)
        {
            BitArray num1, num2;

            if (left.Length > right.Length)
            {
                num1 = new BitArray(left);
                num2 = new BitArray(right);
            }
            else
            {
                num1 = new BitArray(right);
                num2 = new BitArray(left);
            }

            var max = num1.Count;
            var min = num2.Count;
            var result = new BitArray(num1);
            var tmp = false;
            for (var i = max / 8; i > (max - min) / 8; i--)
            {
                for (var j = 8; j > 0; j--)
                {
                    var w = i * 8 - j;
                    var t = (i - (max - min) / 8) * 8 - j;
                    if (num1[w] == num2[t] && num2[t] == tmp)
                    {
                        result[i * 8 - 1] = tmp;
                    }
                    else if (!(num1[w] ^ num2[t] ^ tmp))
                    {
                        result[w] = false;
                        tmp = true;
                    }
                    else if (num1[w] ^ num2[t] ^ tmp)
                    {
                        result[w] = true;
                        tmp = false;
                    }
                }
            }

            if (tmp)
            {
                for (var i = (max - min) / 8; i > 0; i--)
                {
                    for (var j = 8; j > 0; j--)
                    {
                        var w = i * 8 - j;
                        if (num1[w])
                        {
                            result[w] = false;
                        }
                        else
                        {
                            result[w] = true;
                            return BitArrayToByteArray(result);
                        }
                    }
                }
            }

            return BitArrayToByteArray(result);
        }

        public static byte[] GenerateRandomByteArray(int size)
        {
            var array = new byte[size];
            var random = RandomNumberGenerator.Create();
            random.GetBytes(array);
            //делаем число неотр
            array[^1] &= 0b01111111;
            return array;
        }
    }
}