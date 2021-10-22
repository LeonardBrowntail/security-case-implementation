using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FinalProject
{
    class SymetricEncrypt
    {
        private ICryptoTransform transform;
        private AesCryptoServiceProvider aes;
        private byte[] key;
        private byte[] iv;

        public byte[] Key { get => key; }
        public byte[] IV { get => IV}

        public SymetricEncrypt()
        {
            aes = new AesCryptoServiceProvider();
            key = aes.Key;
            iv = aes.IV;
        }

        public byte[] AESEncrypt(byte[] src, byte[] key)
        {
            byte[] encryptedData;
            var 
            using (var AES = aes)
            {
                AES.
            }
            return encryptedData;
        }
    }
}
