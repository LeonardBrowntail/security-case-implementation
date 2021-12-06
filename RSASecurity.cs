using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FinalProject
{
    internal class RSASecurity
    {
        private RSACryptoServiceProvider rsa;
        private RSAParameters foreignPublicKey;

        public RSAParameters PublicKey { get => foreignPublicKey; }

        public byte[] RSAEncrypt(byte[] src, RSAParameters key)
        {
            try
            {
                byte[] encryptedData;
                using (var RSA = rsa)
                {
                    RSA.ImportParameters(key);

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

        public byte[] RSADecrypt(byte[] src, RSAParameters key)
        {
            try
            {
                byte[] decryptedData;
                using (var RSA = rsa)
                {
                    RSA.ImportParameters(key);
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

        public string GetPublicKey()
        {
            return rsa.ToXmlString(false);
        }
    }
}