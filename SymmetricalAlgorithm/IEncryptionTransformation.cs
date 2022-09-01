namespace SymmetricalAlgorithm
{
    public interface IEncryptionTransformation
    {
        public byte[] Transform(byte[] block, byte[] roundKey);
    }
}