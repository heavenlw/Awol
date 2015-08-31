using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Awol
{
    public static class SecurityHelper
    {
        /// <summary>
        /// 生成md5 hash
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GenerateMD5Hash(string text)
        {
            byte[] result = Encoding.Default.GetBytes(text);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);

            return BitConverter.ToString(output).Replace("-", "");
        }
        /// <summary>
        /// Rsa算法加密
        /// </summary>
        /// <param name="text">加密内容</param>
        /// <param name="publicKey">Key</param>
        /// <returns></returns>
        public static string RsaEncrypt(string text, string publicKey)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes = enc.GetBytes(text);

            RSACryptoServiceProvider cryptEncrypt = new RSACryptoServiceProvider();
            cryptEncrypt.FromXmlString(publicKey);
            bytes = cryptEncrypt.Encrypt(bytes, false);
            string encryttext = Convert.ToBase64String(bytes);

            return encryttext;
        }
        /// <summary>
        /// Rsa解密算法
        /// </summary>
        /// <param name="encrytText">加密后的内容</param>
        /// <param name="privateKey">Key</param>
        /// <returns></returns>
        public static string RsaDecrypt(string encrytText, string privateKey)
        {
            RSACryptoServiceProvider crypt = new RSACryptoServiceProvider();

            byte[] bytes = Convert.FromBase64String(encrytText);
            crypt.FromXmlString(privateKey);
            byte[] decryptByte = crypt.Decrypt(bytes, false);

            UTF8Encoding enc = new UTF8Encoding();
            string decryptText = enc.GetString(decryptByte);


            return decryptText;
        }
    }
}
