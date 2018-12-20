using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    public class IO
    {
        public static byte[] BigEndianToLittleEndian(byte[] ar)
        {
            byte[] zero = { 0x00 };
            return ar.Reverse().Concat(zero).ToArray();
        }

        public static byte[] LittleEndianToBigEndian(byte[] ar)
        {
            return ar.Reverse().ToArray();
        }

        public static byte[] AppendByteArray(byte[] arr, int size = 32)
        {
            if (arr.Count() >= size)
                return arr;

            byte[] res = new byte[size];
            for (int i = 0; i < size - arr.Count(); i++)
                res[i] = 0;
            arr.CopyTo(res, size - arr.Count());
            return res;
        }

        public static byte[] CutByteArray(byte[] arr, int size = 8)
        {
            if (arr.Count() <= size)
                return arr;

            byte[] res = new byte[8];
            for (int i = 0; i < 8; i++)
                res[i] = arr[i + 1];
            return res;
        }

        public static void WriteByteArrayToFile(FileStream file, byte[] data)
        {
            for (int i = 0; i < data.Count(); i++)
                file.WriteByte(data[i]);
        }

        public static void MakePadding(ref List<byte> array)
        {
            int offset = 8 - array.Count % 8;
            byte addByte = Convert.ToByte(offset);

            if (offset != 8)
                for (int i = 0; i < offset; i++)
                    array.Add(addByte);
        }

        public static void PrintByteArray(List<byte> array)
        {    
            foreach (byte symbol in array)
                Console.Write(symbol.ToString() + " ");
            Console.WriteLine(" / " + array.Count.ToString() + "b");
        }
    }
}
