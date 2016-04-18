// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The encryption helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------



using System.IO;
using System.Security.Cryptography;

namespace MyApplication.Encryption
{
    /// <summary>The encryption helper.</summary>
    public static class EncryptHelper
    {
        /// <summary>Create and initialize a crypto algorithm.</summary>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="SymmetricAlgorithm"/>.</returns>
        private static SymmetricAlgorithm GetAlgorithm(string password)
        {
            var algorithm = Rijndael.Create();
            var rdb = new Rfc2898DeriveBytes(
                password, 
                new byte[]
                    {
                        0x53, 0x6f, 0x64, 0x69, 0x75, 0x6d, 0x20, // salty goodness
                        0x43, 0x68, 0x6c, 0x6f, 0x72, 0x69, 0x64, 0x65
                    });
            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Key = rdb.GetBytes(32);
            algorithm.IV = rdb.GetBytes(16);
            return algorithm;
        }


        /// <summary>Encrypts a string with a given password.</summary>
        /// <param name="clearBytes">The clear Bytes.</param>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] Encrypt(byte[] clearBytes, string password)
        {
            var algorithm = GetAlgorithm(password);
            var encryptor = algorithm.CreateEncryptor();
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
                return ms.ToArray();
            }
        }

        /// <summary>Decrypts a string using a given password.</summary>
        /// <param name="cipherBytes">The cipher Bytes.</param>
        /// <param name="password">The password.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        public static byte[] Decrypt(byte[] cipherBytes, string password)
        {
            var algorithm = GetAlgorithm(password);
            var decryptor = algorithm.CreateDecryptor();
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
                return ms.ToArray();
            }
        }
    }
}
