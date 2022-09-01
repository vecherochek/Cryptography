namespace SymmetricalAlgorithm
{
    public interface ISymmetricalAlgorithm
    {
        public int BlockSize { get; }
        public byte[] Encrypt(byte[] block, byte[][] roundKeys);
        public byte[] Decrypt(byte[] block, byte[][] roundKeys);
        public byte[][] GenerateRoundKeys(byte[] key);
    }
}