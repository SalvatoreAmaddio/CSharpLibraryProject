using System.Security.Cryptography;

namespace Backend.Utils
{
    /// <summary>
    /// This class encrypts and decrypts strings.
    /// </summary>
    /// <param name="str">The string to descrypt or encrypt</param>
    public class Encrypter(string str)
    {
        private byte[]? key;
        private byte[]? initVector;
        private string Str { get; set; } = str;
        
        /// <summary>
        /// It reads any stored Key's credential and IV's credential.
        /// </summary>
        /// <param name="keyTarget"></param>
        /// <param name="ivTarget"></param>
        public void ReadStoredKeyIV(string keyTarget, string ivTarget) 
        {
            Credential? keyCredential = CredentialManager.Get(keyTarget);
            Credential? ivCredential = CredentialManager.Get(ivTarget);
            if (keyCredential != null) 
                key = Convert.FromBase64String(keyCredential.Password);
            if (ivCredential != null)
                initVector = Convert.FromBase64String(ivCredential.Password);
        }

        /// <summary>
        /// It replaced the stored IV's credential.
        /// </summary>
        /// <param name="target"></param>
        public void ReplaceStoredIV(string target)
        {
            if (CredentialManager.Exist(target)) CredentialManager.Delete(target);
            StoreIV(target);
        }

        /// <summary>
        /// It replaced the stored Key's credential.
        /// </summary>
        /// <param name="target"></param>
        public void ReplaceStoredKey(string target)
        {
            if (CredentialManager.Exist(target)) CredentialManager.Delete(target);
            StoreKey(target);
        }

        /// <summary>
        /// Stores the Credential's key in the local computer.
        /// </summary>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void StoreKey(string target) 
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            CredentialManager.Store(new(target, "key", Convert.ToBase64String(key)));
        }

        /// <summary>
        /// Stores the Credential's IV in the local computer.
        /// </summary>
        /// <param name="target"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void StoreIV(string target)
        {
            if (initVector == null) throw new ArgumentNullException(nameof(initVector));
            CredentialManager.Store(new(target, "iv", Convert.ToBase64String(initVector)));
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <returns>The encrypted string</returns>
        public string Encrypt() 
        {
            using (Aes aes = Aes.Create())
            {
                key ??= aes.Key;
                initVector ??= aes.IV;
                return EncryptString(key, initVector);
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
                key ??= aes.Key;
                initVector ??= aes.IV;
                return DecryptString(key, initVector);
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