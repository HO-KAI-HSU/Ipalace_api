using System.Security.Cryptography;
using System.Text;

namespace Web.Core
{
    public static class AES
    {
        private static readonly byte[] EncryptKey = Encoding.UTF8.GetBytes("BPPYS8p5#@/H,p7jV?c+c8=!vj+ztPQK*=/vT_.n)WLN3~We-R//{#Hh+cG=R,nuZ^Ea-keXXG;gyGcc%<)3Nh!gk~<T#{4u:@m6~gcNE=M3jPV%rwy4E[3F.D&JAA4~");

        public static string Encrypt(string text)
        {
            var sourceBytes = System.Text.Encoding.UTF8.GetBytes(text);
            var aes = System.Security.Cryptography.Aes.Create();
            aes.Mode = System.Security.Cryptography.CipherMode.CBC;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            aes.Key = sha256.ComputeHash(EncryptKey);
            aes.IV = md5.ComputeHash(EncryptKey);
            var transform = aes.CreateEncryptor();
            return System.Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
        }

        public static string Decrypt(string text)
        {
            var encryptBytes = System.Convert.FromBase64String(text);
            var aes = System.Security.Cryptography.Aes.Create();
            aes.Mode = System.Security.Cryptography.CipherMode.CBC;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            aes.Key = sha256.ComputeHash(EncryptKey);
            aes.IV = md5.ComputeHash(EncryptKey);
            var transform = aes.CreateDecryptor();
            return System.Text.Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
        }
    }
}
