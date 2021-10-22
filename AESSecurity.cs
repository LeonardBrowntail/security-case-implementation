using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace FinalProject
{
    class AESSecurity
    {
        private byte[] key;
        private byte[] iv;

        public byte[] Key { get => key; }
        public byte[] IV { get => iv; }

        /// <summary>
        /// This function is used only by the Server to generate new key and IV.
        /// </summary>
        public void GenerateAESKey()
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                aes.GenerateKey();
                key = aes.Key;
                aes.GenerateIV();
                iv = aes.IV;

            }
        }

        /// <summary>
        /// Encrypt a string using instance key and IV.
        /// </summary>
        /// <param name="src">String to be encrypted</param>
        /// <returns>An array of encrypted bytes</returns>
        public byte[] AESEncrypt(string src)
        {
            byte[] encrypted;   
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);  
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {   
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(src);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        /// <summary>
        /// Decrypt a byte array using instance key and IV.
        /// </summary>
        /// <param name="src"></param>
        /// <returns>A string of decrypted plain text</returns>
        public string AESDecrypt(byte[] src)
        {
            string plaintext = null;  
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream ms = new MemoryStream(src))
                {   
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        public void ImplementAESKey(byte[] key, byte[] iv)
        {
            this.key = key;
            this.iv = iv;
        }
    }
}
