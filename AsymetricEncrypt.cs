using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FinalProject
{
    class AsymetricEncrypt
    {
        private RSACryptoServiceProvider rsa;
        private string publicKey;

        public string PublicKey { get => publicKey; }

        public AsymetricEncrypt()
        {
            rsa = new RSACryptoServiceProvider(1024);
            publicKey = rsa.ToXmlString(false);
        }

        public byte[] RSAEncrypt(byte[] src, string key)
        {
            try
            {
                byte[] encryptedData;
                using (var RSA = rsa)
                {
                    RSA.FromXmlString(key);

                    encryptedData = RSA.Encrypt(src, false);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        public byte[] RSADecrypt(byte[] src, string key)
        {
            try
            {
                byte[] decryptedData;
                using (var RSA = rsa)
                {
                    RSA.FromXmlString(key);  
                    decryptedData = RSA.Decrypt(src, false);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }
    }
}
