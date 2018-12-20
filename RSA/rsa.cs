using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RSA
{
    class rsa
    {
        private static Random random = new Random();

        static void Main(string[] args)
        {
            Encrypt("../../rhomb0000.png", "../../output.txt");
            //Decrypt("../../chori.txt", "../../ decrypt.jpeg");
        }

        private static void Decrypt(string filename, string outname)
        {
            List<byte> fileBytes = File.ReadAllBytes(filename).ToList();
            FileStream file = new FileStream(outname, FileMode.Create, FileAccess.Write);
            BigInteger n = 0, d = 0;

            for (int i = 0; i < fileBytes.Count(); i += 32)
            {
                byte[] numberBytes = new byte[32];
                for (int j = 0; j < 32; j++)
                    numberBytes[j] = fileBytes[i + j];

                numberBytes = BigToLittle(numberBytes);

                if (i == 0)
                    d = new BigInteger(numberBytes);
                else if (i == 32)
                    n = new BigInteger(numberBytes);
                else
                {
                    BigInteger c = new BigInteger(numberBytes);
                    c = BigInteger.ModPow(c, d, n);
                    numberBytes = LittleToBig(c.ToByteArray());

                    if (numberBytes.Count() < 8)
                        numberBytes = Append(numberBytes, 8);
                    else if (numberBytes.Count() > 8)
                        numberBytes = Cut(numberBytes);

                    WriteToFile(file, numberBytes);
                }
            }
            file.Close();
        }

        private static void Encrypt(string filename, string outname)
        {
            List<byte> fileBytes = File.ReadAllBytes(filename).ToList();
            MakePadding(ref fileBytes);

            BigInteger p, q, n, carmichael, e, d;
            do
            {
                p = RandomPrimeNumber();
                q = RandomPrimeNumber();
                n = ComputeN(p, q);
                carmichael = ComputeCarmichael(p, q);            
                e = ComputeE(carmichael);
                d = ComputeD(e, carmichael);
            } while ((d.ToByteArray().Count() < 32) || (e * d % carmichael != 1));

            FileStream file = new FileStream(outname, FileMode.Create, FileAccess.ReadWrite);

            byte[] ar = LittleToBig(d.ToByteArray());
            ar = Append(ar);
            WriteToFile(file, ar);

            ar = LittleToBig(n.ToByteArray());
            ar = Append(ar);
            WriteToFile(file, ar);

            for (int i = 0; i < fileBytes.Count(); i += 8)
            {
                byte[] numberBytes = new byte[8];
                for (int j = 0; j < 8; j++)
                    numberBytes[j] = fileBytes[i + j];

                numberBytes = BigToLittle(numberBytes);
                
                BigInteger number = new BigInteger(numberBytes);

                number = BigInteger.ModPow(number, e, n);
                numberBytes = LittleToBig(number.ToByteArray());
                if (numberBytes.Count() < 32)
                    numberBytes = Append(numberBytes);

                WriteToFile(file, numberBytes);
            }
            file.Close();
        }

        private static byte[] BigToLittle(byte[] ar)
        {
            byte[] zero = { 0x00 };
            return ar.Reverse().Concat(zero).ToArray();
        }

        private static byte[] LittleToBig(byte[] ar)
        {
            return ar.Reverse().ToArray();
        }

        private static byte[] Append(byte[] arr, int size = 32)
        {
            if (arr.Count() == size)
                return arr;

            byte[] res = new byte[size];
            for (int i = 0; i < size - arr.Count(); i++)
                res[i] = 0;
            arr.CopyTo(res, size - arr.Count());
            return res;
        }

        private static byte[] Cut(byte[] arr)
        {
            byte[] res = new byte[8];
            for (int i = 0; i < 8; i++)
                res[i] = arr[i + 1];
            return res;
        }

        private static void WriteToFile(FileStream file, byte[] data)
        {
            for (int i = 0; i < data.Count(); i++)
                file.WriteByte(data[i]);
        }

        private static void MakePadding(ref List<byte> array)
        {
            int offset = 8 - array.Count % 8;
            byte addByte = Convert.ToByte(offset);

            if (offset != 8)
                for (int i = 0; i < offset; i++)
                    array.Add(addByte);
        }

        private static void PrintByteArray(List<byte> array)
        {
            Console.WriteLine("Length: " + array.Count.ToString());
            foreach (byte symbol in array)
                Console.Write(symbol.ToString() + " ");
            Console.WriteLine();
        }

        private static bool FermatTest(BigInteger n, int accuracy = 1000)
        {
            BigInteger a = RandomNumber(random.Next(2, n.ToByteArray().Count()));
            for (int i = 0; i < accuracy; i++)
                if (BigInteger.ModPow(a, n - 1, n) != 1)
                    return false;
            return true;
        }

        private static bool MillerRabinTest(BigInteger n, int accuracy = 1000)
        {
            if (n % 2 == 0)
                return false;

            BigInteger d = n - 1, r = 0;
            while (d % 2 == 0)
            {
                r++;
                d /= 2;
            }

            for (int i = 0; i < accuracy; i++)
            {
                BigInteger a = RandomNumber(random.Next(2, n.ToByteArray().Count()));
                BigInteger x = 1;
                x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == n - 1)
                    continue;
                bool flag = false;
                for (int j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == n - 1)
                    {
                        flag = true;
                        break;
                    }
                    if (flag)
                        continue;
                    return false;
                }
            }
            return true;
        }

        private static BigInteger RandomNumber(int bytelength)
        {
            byte[] data = new byte[bytelength];
            random.NextBytes(data);
            return BigInteger.Abs(new BigInteger(data));
        }

        private static BigInteger RandomPrimeNumber(int bytelength = 16)
        {
            byte[] data = new byte[bytelength];
            BigInteger res = RandomNumber(bytelength);
            do
                res = RandomNumber(bytelength);
            while (!FermatTest(res) || !MillerRabinTest(res));
            return res;
        }

        private static BigInteger ComputeN(BigInteger p, BigInteger q)
        {
            return BigInteger.Multiply(p, q);
        }

        private static BigInteger ComputeCarmichael(BigInteger p, BigInteger q)
        {
            BigInteger left = p - 1, right = q - 1;
            return BigInteger.Multiply(left, right) / BigInteger.GreatestCommonDivisor(left, right);
        }

        private static BigInteger ComputeE(BigInteger carmichael)
        {
            BigInteger e;
            do
                e = RandomNumber(random.Next(7, carmichael.ToByteArray().Count()));
            while (BigInteger.GreatestCommonDivisor(e, carmichael) != 1);
            return e;
        }

        private static BigInteger ComputeD(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = 0, x = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                BigInteger q = a / m;
                BigInteger t = m;

                m = a % m;
                a = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            return x;
        }

    }
}
