using System;
using System.Linq;
using Cryptography.Extensions;

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
            var (left, right) = (block.Take(block.Length / 2).ToArray(), block.Skip(block.Length / 2).ToArray());
            
            var numberOfRounds = 6;
            if (_key.Length == 32) numberOfRounds = 8;
            
            for (var i = 0; i < numberOfRounds; i++)
            {
                var des = new DES.DES(_roundKeys[i], DES.EncryptionModes.CBC, new byte[]{1,1,1,1,1,1,1,1});
                var tmp = left;
                left = des.EncryptBlock(left).Xor(right);
                right = tmp;
            }
            
            return left.Concat(right).ToArray();
        }
        private byte[] Decrypt (byte[] block) 
        {

            var (left, right) = (block.Take(block.Length / 2).ToArray(), block.Skip(block.Length / 2).ToArray());
            
            var numberOfRounds = 6;
            if (_key.Length == 32) numberOfRounds = 8;
            
            for (var i = 0; i < numberOfRounds; i++)
            {
                var des = new DES.DES(_roundKeys[i], DES.EncryptionModes.CBC, new byte[]{1,1,1,1,1,1,1,1});
                var tmp = right;
                right = des.EncryptBlock(right).Xor(left);
                left = tmp;
            }
            
            return left.Concat(right).ToArray();
        }
    }
}