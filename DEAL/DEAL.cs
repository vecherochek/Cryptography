using System;
using System.Numerics;

namespace DEAL
{
    public class DEAL
    {
        private readonly byte[][] _roundKeys;
        private readonly byte[] _key;

        
        public DEAL(byte[] key)
        {
            _key = key;
            _roundKeys = new KeysGenerator(_key).GenerateRoundKeys();
        }
        public byte[] EncryptBlock(byte[] message)
        {
            var original = PaddingPKCs7(message);
            var result = new byte[original.Length];
            
            for (var i = 0; i < result.Length / 16; i++)
            {
                var currentBlock = new byte[16];
            
                Array.Copy(original, i * 16, currentBlock, 0, 16);
                currentBlock = Encrypt(currentBlock);
                Array.Copy(currentBlock, 0, result, i * 16, 16);
            }
            return result;
        }
        public byte[] DecryptBlock(byte[] message)
        {
            var result = new byte[message.Length];

            for (var i = 0; i < result.Length / 16; i++)
            {
                var currentBlock = new byte[16];

                Array.Copy(message, i * 16, currentBlock, 0, 16);
                currentBlock = Decrypt(currentBlock);
                Array.Copy(currentBlock, 0, result, i * 16, 16);
            }

            Array.Resize(ref result, message.Length - result[^1]);
            
            return result;
        }
        private static byte[] PaddingPKCs7(byte[] block)
        {
            byte addition = (byte) (16 - block.Length % 16);
            var paddedBlock = new byte[block.Length + addition];
            Array.Copy(block, paddedBlock, block.Length);
            Array.Fill(paddedBlock, addition, block.Length, addition); 
            
            return paddedBlock;
        }
        private byte[] Encrypt (byte[] block) 
        {

            var number = new BigInteger(block);

            var left = number >> (int) number.GetByteCount() * 4;
            var right = (number & (BigInteger.One << (int) number.GetByteCount() * 4) - BigInteger.One);
            var numberOfRounds = 6;
            if (_key.Length == 32) numberOfRounds = 8;
            
            for (var i = 0; i < numberOfRounds; i++)
            {
                var des = new DES.DES(_roundKeys[i], DES.EncryptionModes.CBC, new byte[]{1,1,1,1,1,1,1,1});
                var tmp = left;
                left = new BigInteger(des.Encrypt(left.ToByteArray()));
                left = left ^ right;
                right = tmp;
            }

            var t = (left << (int) number.GetByteCount() * 4 | right).ToByteArray();
            return t;
        }
        private byte[] Decrypt (byte[] block) 
        {

            var number = new BigInteger(block);

            var left = number >> (int) number.GetByteCount() * 4;
            var right = (number & (BigInteger.One << (int) number.GetByteCount() * 4) - BigInteger.One);
            var numberOfRounds = 6;
            if (_key.Length == 32) numberOfRounds = 8;
            
            for (var i = numberOfRounds - 1; i >= 0; i--)
            {
                var des = new DES.DES(_roundKeys[i], DES.EncryptionModes.CBC, new byte[]{1,1,1,1,1,1,1,1});
                var tmp = right;
                right = new BigInteger(des.Decrypt(right.ToByteArray()));
                right = left ^ right;
                left = tmp;
            }
            var t = (left << (int) number.GetByteCount() * 4 | right).ToByteArray();
            return t;
        }
        
    }
}