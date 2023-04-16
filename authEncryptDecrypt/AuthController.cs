using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISAutoParts.authCodeDecode
{
    public class AuthController
    {
        private static string _encryptedPassword;


        public static string Encrypt(string password)
        {
            _encryptedPassword = "";
            for (int i = 0; i < password.Length; i++)
            {
                char p = password[i];
                var numberAscii = (int)p;
                numberAscii += 10;
                _encryptedPassword += Char.ConvertFromUtf32(numberAscii);
            }
            return _encryptedPassword;
        }


        public static string Decrypt(string password)
        {
            _encryptedPassword = "";
            for (int i = 0; i < password.Length; i++)
            {
                char p = password[i];
                var numberAscii = (int)p;
                numberAscii -= 10;
                _encryptedPassword += Char.ConvertFromUtf32(numberAscii);
            }
            return _encryptedPassword;
        }
    }
}
