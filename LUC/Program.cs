﻿using System;
using System.Numerics;
using System.Diagnostics;
using System.Text;

namespace LUC
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            
            //BigInteger message = new BigInteger(Encoding.Default.GetBytes("кекекекекекеекекекеекекеекекек"));
            BigInteger message = new BigInteger(Encoding.Default.GetBytes("11444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444444"));
            Console.WriteLine("Размер сообщения в байтах: " + message.GetByteCount());
            
            stopwatch.Start();
            var a = new KeyGenerator(message);
            stopwatch.Stop();
            
            Console.WriteLine("Время ген ключа: "+ stopwatch.Elapsed.TotalSeconds + " сек");
            stopwatch.Reset();
            
            stopwatch.Start();
            var en = new LUC().Encrypt(message, a.PublicKey);
            stopwatch.Stop();
            
            Console.WriteLine("Время шифр: "+ stopwatch.Elapsed.TotalSeconds + " сек");
            stopwatch.Reset();
            
            stopwatch.Start();
            BigInteger decrypt = new LUC().Decrypt(message, a.PrivateKey);
            stopwatch.Stop();
            Console.WriteLine("Время дешифр: "+ stopwatch.Elapsed.TotalSeconds + " сек");
            Console.WriteLine("Расшифр: " +  Encoding.Default.GetString(decrypt.ToByteArray()));
        }
    }
}