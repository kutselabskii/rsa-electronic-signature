using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSA
{
    class Start
    {
        static void Main(string[] args)
        {        
            RSA.Encrypt("../../rhomb0000.png", "../../output.txt");
            RSA.Decrypt("../../output.txt", "../../ decrypt.png");
        }
    }
}
