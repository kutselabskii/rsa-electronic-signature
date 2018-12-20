using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace RSA
{
    class RSA
    {

        public static void Encrypt(string filename, string outname)
        {
            List<byte> fileBytes = File.ReadAllBytes(filename).ToList();
            IO.MakePadding(ref fileBytes);

            BigInteger p, q, n, carmichael, e, d;
            do
            {
                p = Math.RandomPrimeNumber();
                q = Math.RandomPrimeNumber();
                n = p * q;
                carmichael = Math.RSA.ComputeCarmichael(p, q);
                e = Math.RSA.ComputeE(carmichael);
                d = Math.RSA.ComputeD(e, carmichael);
            } while ((d.ToByteArray().Count() < 32) || (e * d % carmichael != 1));

            FileStream file = new FileStream(outname, FileMode.Create, FileAccess.ReadWrite);

            IO.WriteByteArrayToFile(file, IO.AppendByteArray(IO.LittleEndianToBigEndian(d.ToByteArray())));
            IO.WriteByteArrayToFile(file, IO.AppendByteArray(IO.LittleEndianToBigEndian(n.ToByteArray())));

            for (int i = 0; i < fileBytes.Count(); i += 8)
            {
                byte[] numberBytes = new byte[8];
                for (int j = 0; j < 8; j++)
                    numberBytes[j] = fileBytes[i + j];

                numberBytes = IO.BigEndianToLittleEndian(numberBytes);

                BigInteger number = new BigInteger(numberBytes);

                number = BigInteger.ModPow(number, e, n);
                numberBytes = IO.LittleEndianToBigEndian(number.ToByteArray());
                numberBytes = IO.AppendByteArray(numberBytes);

                IO.WriteByteArrayToFile(file, numberBytes);
            }
            file.Close();
        }

        public static void Decrypt(string filename, string outname)
        {
            List<byte> fileBytes = File.ReadAllBytes(filename).ToList();
            FileStream file = new FileStream(outname, FileMode.Create, FileAccess.Write);
            BigInteger n = 0, d = 0;

            for (int i = 0; i < fileBytes.Count(); i += 32)
            {
                byte[] numberBytes = new byte[32];
                for (int j = 0; j < 32; j++)
                    numberBytes[j] = fileBytes[i + j];
                
                numberBytes = IO.BigEndianToLittleEndian(numberBytes);

                if (i == 0)
                    d = new BigInteger(numberBytes);
                else if (i == 32)
                    n = new BigInteger(numberBytes);
                else
                {
                    BigInteger c = new BigInteger(numberBytes);
                    c = BigInteger.ModPow(c, d, n);
                    numberBytes = IO.LittleEndianToBigEndian(c.ToByteArray());

                    numberBytes = IO.AppendByteArray(numberBytes, 8);
                    numberBytes = IO.CutByteArray(numberBytes, 8);

                    IO.WriteByteArrayToFile(file, numberBytes);
                }
            }
            file.Close();
        }

    }
}
