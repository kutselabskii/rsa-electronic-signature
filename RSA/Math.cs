using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;

namespace RSA
{
    public class Math
    {
        private static readonly Random random = new Random();

        public static BigInteger RandomNumber(int bytelength)
        {
            byte[] data = new byte[bytelength];
            random.NextBytes(data);
            return BigInteger.Abs(new BigInteger(data));
        }

        public static bool FermatTest(BigInteger n, int accuracy = 1000)
        {
            BigInteger a = RandomNumber(random.Next(2, n.ToByteArray().Count()));
            for (int i = 0; i < accuracy; i++)
                if (BigInteger.ModPow(a, n - 1, n) != 1)
                    return false;
            return true;
        }

        public static bool MillerRabinTest(BigInteger n, int accuracy = 1000)
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

        public static BigInteger RandomPrimeNumber(int bytelength = 16)
        {
            byte[] data = new byte[bytelength];
            BigInteger res = RandomNumber(bytelength);
            do
                res = RandomNumber(bytelength);
            while (!FermatTest(res) || !MillerRabinTest(res));
            return res;
        }

        public static class RSA
        {
            public static BigInteger ComputeN(BigInteger p, BigInteger q)
            {
                return p * q;
            }

            public static BigInteger ComputeCarmichael(BigInteger p, BigInteger q)
            {
                BigInteger left = p - 1, right = q - 1;
                return (left * right) / BigInteger.GreatestCommonDivisor(left, right);
            }

            public static BigInteger ComputeE(BigInteger carmichael)
            {
                BigInteger e;
                do
                    e = RandomNumber(random.Next(7, carmichael.ToByteArray().Count()));
                while (BigInteger.GreatestCommonDivisor(e, carmichael) != 1);
                return e;
            }

            public static BigInteger ComputeD(BigInteger a, BigInteger m)
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
}
