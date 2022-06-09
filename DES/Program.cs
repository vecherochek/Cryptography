using System;
using System.Text;

namespace DES
{
    class Program
    {
        static void Main(string[] args)
        {
            ulong a =  0x1234567890abcdef;
            
            string b = "кекеcszdvfsgbetyw665464   /*- ./..,,/7 rtr    45yt45t";
            
            byte[] key = BitConverter.GetBytes(a);
            
            byte[] block = Encoding.Default.GetBytes(b);
            
            var t = new DES(key, EncryptionModes.CBC, new byte[]{1,1,1,0,1,1,1,1});
            var en = t.EncryptBlock(block);
            
            //Console.WriteLine(new BigInteger(en));
            Console.WriteLine(Encoding.Default.GetString(t.DecryptBlock(en))); 
            //var Number =(BigInteger)Math.Pow(2, 63) - 5236346456524;
            //Console.WriteLine(Number.GetByteCount());
            //Console.WriteLine(Number);

        }
    }
}