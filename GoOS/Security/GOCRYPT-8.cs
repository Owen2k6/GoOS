using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoOS.Security
{
    internal class GOCRYPT_8
    {
        static string Encrypt(string password)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);
            byte[] result = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ 42); // XOR with a constant value
            }

            return Convert.ToBase64String(result);
        }

        static string Decrypt(string encryptedPassword)
        {
            byte[] data = Convert.FromBase64String(encryptedPassword);
            byte[] result = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ 42); // XOR with a constant value
            }

            return Encoding.UTF8.GetString(result);
        }
    }
}
