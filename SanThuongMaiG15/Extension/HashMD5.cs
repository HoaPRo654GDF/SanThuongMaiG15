using System.Security.Cryptography;
using System.Text;
using System;

namespace SanThuongMaiG15.Extension
{
    public static class HashMD5
    {
        public static string ToMD5(this string str)
        {

             MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
             byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
             StringBuilder sbhash = new StringBuilder();
             foreach (byte b in bHash)

                sbhash.Append(String.Format("{0:x2}", b));

             return sbhash.ToString();

        }
    }
}
