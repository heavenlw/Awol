using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace App.Utilities
{
    public static class SecurityHelper
    {
        public static bool VerifyRasKey(string encrytText)
        {
            bool result = false;

            string privateKey = @"<RSAKeyValue><Modulus>xJrxw5tlnwPYM+CiE6BuMJRZJbrUQlmd+jV4khMRJ7nzDSAMeX/J+YWhSePHww6p0koz4NsUWCT5x5LJZ3s4KGdjNwRRA8dxCL80xWD79KaI+hQcR247Kh3AKS1XatwshW/1QQoQ1Dwg4JCrS/VdxYUgrSO2WCmkaEiDWhcbe28=</Modulus><Exponent>AQAB</Exponent><P>56p7Z27RkEpwd5W2mUle32gbK9u2PzIFgbnxKb+NoX6Uw/h1RSnER8VB4KvMNFm+l/lT9btpeKmE+aZm/Wm6cw==</P><Q>2UGtWfBilw16SzojZ1Z0kaThUF11vQyE/EPM6PX5w4v0ZOd9TPqn6C528uAUL92OTQ+HmaS79SqRadfbLRgQFQ==</Q><DP>lqkTmj/CwCD5JXxTBTtnHMl6qjo4Or8QP76qbSkrNbS5kP07XuB7yuUpI7D2m7Elt3YpuSzJufQdC7LBVdr1qw==</DP><DQ>k2HtoqTjjQt0mhHvsIvC+oa63xT36W7TzHqGSMeNT23jNoyfwRgNzgGvaeY/a5VGktplKALMC258RSxNIJNBXQ==</DQ><InverseQ>YbBUSau+1NKSPmHWaah1ELFxzupu0yxErOEC2iPKi5fDXjChFmc7gvNS9RH8Hi7bLEaxWcwae4XW4SsQPc3x+w==</InverseQ><D>lVUne68mIgE2kDj4grXh3G5hxEHDhd4yG2HQAgwPhMA99+M29nZ1AF1a5BfqnKMfpIYOH6XoLwdu3gMFjd9PCDR6y89XgqAVU+xo70vSzrqkQjvDGg6bltcvQlEJVBAiNU/8xAxc+a7+AWgqXHb05LydOuhUDNcRZZLyKnjhSDk=</D></RSAKeyValue>";

            string decryptText = RsaDecrypt(encrytText, privateKey);

            decryptText = decryptText.Split(';')[0];

            if (decryptText == "
App")
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 生成MD5哈希值
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

        public static string GenerateRasEncryptText()
        {
            string publicKey = @"<RSAKeyValue><Modulus>xJrxw5tlnwPYM+CiE6BuMJRZJbrUQlmd+jV4khMRJ7nzDSAMeX/J+YWhSePHww6p0koz4NsUWCT5x5LJZ3s4KGdjNwRRA8dxCL80xWD79KaI+hQcR247Kh3AKS1XatwshW/1QQoQ1Dwg4JCrS/VdxYUgrSO2WCmkaEiDWhcbe28=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

            string encryptText = RsaEncrypt("App;" + DateTime.Now.Ticks.ToString(), publicKey);

            return encryptText;
        }

        public static string RsaEncrypt(string text,string publicKey)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes = enc.GetBytes(text);

            RSACryptoServiceProvider cryptEncrypt = new RSACryptoServiceProvider();
            cryptEncrypt.FromXmlString(publicKey);
            bytes = cryptEncrypt.Encrypt(bytes, false);
            string encryttext = Convert.ToBase64String(bytes);

            return encryttext;
        }

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
