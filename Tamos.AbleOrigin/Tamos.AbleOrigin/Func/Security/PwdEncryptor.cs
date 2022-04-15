using System;
using System.Security.Cryptography;

namespace Tamos.AbleOrigin
{
    /// <summary>
    /// Password hash加密
    /// </summary>
    public static class PwdEncryptor
    {
        private const int V1_SaltSize = 128/8; // 128 bits
        private const int V1_SubkeyLength = 256/8; // 256 bits
        private const int V1_IterCount = 1000; // Hash 循环次数，default for Rfc2898DeriveBytes

        /// <summary>
        /// 使用Rfc2898DeriveBytes(PBKDF2)算法进行哈希，结果长度：65
        /// </summary>
        public static string HashPasswordV1(string pwd)
        {
            if (pwd.IsNull()) return string.Empty;

            using var deriveBytes = new Rfc2898DeriveBytes(pwd, V1_SaltSize, V1_IterCount, HashAlgorithmName.SHA256);
            var salt = deriveBytes.Salt;
            var subkey = deriveBytes.GetBytes(V1_SubkeyLength);

            //Result format: {version}{salt}{subkey}
            var outputBytes = new byte[V1_SaltSize + V1_SubkeyLength];
            salt.CopyTo(outputBytes, 0);
            subkey.CopyTo(outputBytes, V1_SaltSize);
            return "1" + Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// 验证密码输入
        /// </summary>
        public static bool VerifyPassword(string inputPwd, string hashedPwd)
        {
            if (inputPwd.IsNull() || hashedPwd.IsNull() || hashedPwd.Length <= 1) return false;

            //-- Version-1 verify
            var decodedHashed = Convert.FromBase64String(hashedPwd[1..]);
            if (decodedHashed.Length != V1_SaltSize + V1_SubkeyLength) return false; // bad size

            var salt = new byte[V1_SaltSize];
            var expectedSubkey = new byte[V1_SubkeyLength];
            Buffer.BlockCopy(decodedHashed, 0, salt, 0, salt.Length);
            Buffer.BlockCopy(decodedHashed, salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the input password
            using var deriveBytes = new Rfc2898DeriveBytes(inputPwd, salt, V1_IterCount, HashAlgorithmName.SHA256);
            var actualSubkey = deriveBytes.GetBytes(V1_SubkeyLength);

            return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
        }
    }
}