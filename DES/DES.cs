using System;
using System.Numerics;

namespace DES
{
    public class DES
    {
        private readonly ulong[] _roundKeys;
        private readonly EncryptionModes _encryptionMode;
        private readonly byte[] _initializationVector;

        public DES(byte[] key, EncryptionModes encryptionMode, byte[] initializationVector)
        {
            _roundKeys = new KeysGenerator(key).GenerateRoundKeys();
            _encryptionMode = encryptionMode;
            _initializationVector = initializationVector;
        }
        
        public byte[] EncryptBlock(byte[] message)
        {
            var original = PaddingPKCs7(message);
            var result = new byte[original.Length];

            switch (_encryptionMode)
            {
                case EncryptionModes.ECB:
                    for (var i = 0; i < result.Length / 8; i++)
                    {
                        var currentBlock = new byte[8];
                
                        Array.Copy(original, i * 8, currentBlock, 0, 8);
                        currentBlock = Encrypt(currentBlock);
                        Array.Copy(currentBlock, 0, result, i * 8, 8);
                    }
                    break;
                
                case EncryptionModes.CBC:
                {
                    var prevBlock = new byte[8];
                    Array.Copy(_initializationVector, prevBlock, prevBlock.Length);
                    for (var i = 0; i < result.Length / 8; i++)
                    {
                        var currentBlock = new byte[8];
                        Array.Copy(original, i * 8, currentBlock, 0, 8);

                        currentBlock = BitConverter.GetBytes(BitConverter.ToUInt64(prevBlock, 0) ^ BitConverter.ToUInt64(currentBlock, 0));
                        currentBlock = Encrypt(currentBlock);
                        
                        Array.Copy(currentBlock, 0, result, i * 8, 8);
                        Array.Copy(currentBlock, prevBlock, prevBlock.Length);
                    }
                }
                    break;
            }
            
            return result;
        }
        
        public byte[] DecryptBlock(byte[] message)
        {
            var result = new byte[message.Length];
            switch (_encryptionMode)
            {
                case EncryptionModes.ECB:
                {
                    for (var i = 0; i < result.Length / 8; i++)
                    {
                        var currentBlock = new byte[8];

                        Array.Copy(message, i * 8, currentBlock, 0, 8);
                        currentBlock = Decrypt(currentBlock);

                        Array.Copy(currentBlock, 0, result, i * 8, 8);
                    }

                    break;
                }
                case EncryptionModes.CBC:
                    {
                        var prevBlock = new byte[8];
                        Array.Copy(_initializationVector, prevBlock, prevBlock.Length);
                        for (var i = 0; i < result.Length / 8; i++)
                        {
                            var currentBlock = new byte[8];
                            var tmp = new byte[8];

                            Array.Copy(message, i * 8, currentBlock, 0, 8);
                            Array.Copy(currentBlock, tmp , tmp.Length);

                            currentBlock = Decrypt(currentBlock);
                            currentBlock = BitConverter.GetBytes(
                                BitConverter.ToUInt64(prevBlock, 0) ^ BitConverter.ToUInt64(currentBlock, 0) );
                            
                            Array.Copy(tmp, prevBlock, prevBlock.Length);
                            Array.Copy(currentBlock, 0, result, i * 8, 8);
                        }
                    }
                    break;
            }
            Array.Resize(ref result, message.Length - result[^1]);
            
            return result;
        }

        public byte[] Encrypt(byte[] block)
        {
            block = Functions.Permutation64(block, Tables.InitialPermutation);
            block = new FeistelNetwork(_roundKeys).Encrypt(block);
            
            return Functions.Permutation64(block, Tables.FinalPermutation);
        }
        public byte[] Decrypt(byte[] block)
        {
            block = Functions.Permutation64(block, Tables.InitialPermutation);
            block = new FeistelNetwork(_roundKeys).Decrypt(block);
            
            return Functions.Permutation64(block, Tables.FinalPermutation);
        }
        public static byte[] PaddingPKCs7(byte[] block)
        {
            byte addition = (byte) (8 - block.Length % 8);
            var paddedBlock = new byte[block.Length + addition];
            Array.Copy(block, paddedBlock, block.Length);
            Array.Fill(paddedBlock, addition, block.Length, addition); 
            
            return paddedBlock;
        }
    }
}