using System.Security.Cryptography;
using System.Text;

namespace Tamos.AbleOrigin
{
    public class Encryptor
    {
        /// <summary>
        /// 对字符串进行MD5加密
        /// </summary>
        public static string MD5Encrypt(string input)
        {
            using var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 对字符串进行Sha1加密
        /// </summary>
        public static string SHA1Encrypt(string input)
        {
            using var sha1 = SHA1.Create();
            var data = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
