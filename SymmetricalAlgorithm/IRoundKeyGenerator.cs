namespace SymmetricalAlgorithm
{
    public interface IRoundKeyGenerator
    {
        public byte[][] GenerateRoundKeys(byte[] key);
    }
}