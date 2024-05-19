using System.Security.Cryptography;

namespace Backend.Utils
{
    /// <summary>
    /// This class encrypts and decrypts strings.
    /// </summary>
    public class Encrypter(string str)
    {
        private string Str { get; set; } = str;

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <returns>The encrypted string</returns>
        public string Encrypt() 
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = aes.Key;
                byte[] iv = aes.IV;
                return EncryptString(key, iv);
            }
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <returns>The string with its original value</returns>
        public string Decrypt()
        {
            using (Aes aes = Aes.Create())
            {
                byte[] key = aes.Key;
                byte[] iv = aes.IV;
                return DecryptString(key, iv);
            }
        }


        private string DecryptString(byte[] key, byte[] iv)
        {
            byte[] buffer = Convert.FromBase64String(Str);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new(buffer))
                using (CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader sr = new(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        private string EncryptString(byte[] key, byte[] iv)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new())
                {
                    using (CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter sw = new(cs))
                    {
                        sw.Write(Str);
                    }

                    byte[] encrypted = ms.ToArray();
                    return Convert.ToBase64String(encrypted);
                }
            }
        }
    }
}