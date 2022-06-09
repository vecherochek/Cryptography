using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace DEAL
{
    class Program
    {
        static void Main(string[] args)
        {
            //BigInteger a =  BigInteger.Parse("3138550867693340381917894711603833208051177716995670799924");
            BigInteger a =  BigInteger.Parse("170141183460469231731687298479537649204");

            
           //Console.WriteLine("a " +  string.Concat(a.ToByteArray().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse()));
           
           
          //BigInteger k1 = BigInteger.Parse("7133142423689677855");
          //var _k1 = string.Concat(k1.ToByteArray().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse());
          //Console.WriteLine("k1 " + _k1);
          //Console.WriteLine(_k1.Length);

          //BigInteger k2 = BigInteger.Parse("-2013637284526028054");
          //var _k2 = string.Concat(k2.ToByteArray().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse());
          //Console.WriteLine("k2 " + _k2);
          //Console.WriteLine(_k2.Length);
          // 
          //var rt = (k1 << (int) 16 * 4 | k2).ToByteArray();
          //var _t = string.Concat(rt.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse());
          //Console.WriteLine("t " + _t);
          //Console.WriteLine(rt.Length);
          //Console.WriteLine(Encoding.Default.GetString(rt));
           //BigInteger k3 = (a >> 64) & ((BigInteger.One << 64) - BigInteger.One);
           //var t = k3.ToByteArray();
           //var _k3 = string.Concat(k3.ToByteArray().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse());
           //Console.WriteLine("k3 " + _k3);
           //Console.WriteLine(_k3.Length);


            //BigInteger k4 = (a >> 128) & ((BigInteger.One << 64) - BigInteger.One);
            //Console.WriteLine("k4 " + string.Concat(k4.ToByteArray().Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse()));
            //var Number =(BigInteger)Math.Pow(2, 127) - 5236346456524;
            //Console.WriteLine(Number.GetByteCount());
            //Console.WriteLine(Number);
            string b = "кек65grw8w-4k4w3495e49-5ugu8te54";
            
            byte[] key = a.ToByteArray();
            
            byte[] block = Encoding.Default.GetBytes(b);
            
            var t = new DEAL(key);
            var en = t.EncryptBlock(block);
            Console.WriteLine(Encoding.Default.GetString(t.DecryptBlock(en)));
        }
    }
}