using System.Security.Cryptography;

namespace Backend.Utils
{
    /// <summary>
    /// This class encrypts and decrypts strings.
    /// </summary>
    public class Encrypter(string str)
    {
        public byte[]? key;
        public byte[]? initVector;
        private string Str { get; set; } = str;
       
        private void ReadStoredKeys() 
        { 
            key = GetStoredKey();
            initVector = GetStoredIV();
        }

        private static byte[]? GetStoredKey() 
        {
            Credential? credential = CredentialManager.Get("EncrypterKEY");
            if (credential == null) return null;
            return Convert.FromBase64String(credential.Password);
        }

        private static byte[]? GetStoredIV()
        {
            Credential? credential = CredentialManager.Get("EncrypterIV");
            if (credential == null) return null;
            return Convert.FromBase64String(credential.Password);
        }

        public void ReplaceStoredIV()
        {
            if (CredentialManager.Exist("EncrypterIV")) CredentialManager.Delete("EncrypterIV");
            StoreKey();
        }

        public void ReplaceStoredKey()
        {
            if (CredentialManager.Exist("EncrypterKEY")) CredentialManager.Delete("EncrypterKEY");
            StoreKey();
        }

        public void StoreKey() 
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            CredentialManager.Store(new("EncrypterKEY", "key", Convert.ToBase64String(key)));
        }

        public void StoreIV()
        {
            if (initVector == null) throw new ArgumentNullException(nameof(initVector));
            CredentialManager.Store(new("EncrypterIV", "iv", Convert.ToBase64String(initVector)));
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <returns>The encrypted string</returns>
        public string Encrypt() 
        {
            ReadStoredKeys();
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
            ReadStoredKeys();
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