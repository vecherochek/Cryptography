using System;
using System.Numerics;
using System.Text;

namespace DEAL
{
    class Program
    {
        static void Main(string[] args)
        {
            BigInteger a =  BigInteger.Parse("3138550867693340381917894711603833208051177716995670799924");
            
            string b = "кекеcszdvfsgbetyw45yt45t";
            
            byte[] key = a.ToByteArray();
            
            byte[] block = Encoding.Default.GetBytes(b);
            
            var t = new DEAL(key);
            var en = t.EncryptBlock(block);
            
            Console.WriteLine(new BigInteger(en));
            Console.WriteLine(Encoding.Default.GetString(t.DecryptBlock(en)));
            
        }
    }
}