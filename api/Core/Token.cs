namespace Web.Core
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class Token
    {
        private static byte[] KEY = Encoding.UTF8.GetBytes("@4XBM-~{zn8crNu@");
        private static byte BYTE_KEY = 0xAF;
        public static TimeSpan ExpireInterval = TimeSpan.FromHours(1);
        public static TimeSpan RenewInterval = TimeSpan.FromMinutes(15);

        private Random rnd = new Random();

        public int AccountID { get; set; }

        public string Name { get; set; }

        public string Extra { get; set; }

        public TokenType Type { get; set; }

        public DateTime ExpireTime { get; set; } = DateTime.Now.Add(ExpireInterval);

        private static string Codes = "1qazQAZ2wsxWSX3edcEDC4rfvRFV5tgbTGB6yhnYHN7ujmUJM8ikIK9olOL0pP";

        private static char ToCode(int index) => Codes[index];

        private static int FromCode(char c) => Codes.IndexOf(c);

        public override string ToString()
        {
            return $"AccountID={AccountID}, Name={Name}, Extra={Extra}, ExpireTime={ExpireTime}";
        }

        private string GetExpireTimeString()
        {
            return new string(new char[] { ToCode(ExpireTime.Year - 2020), ToCode(ExpireTime.Month), ToCode(ExpireTime.Day), ToCode(ExpireTime.Hour), ToCode(ExpireTime.Minute) });
        }

        public bool IsExpired { get => DateTime.Now > ExpireTime; }

        public bool CanRenew { get => DateTime.Now <= ExpireTime.Add(RenewInterval); }

        public string Generate()
        {
            string encrypt = null;
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(KEY);
                byte[] iv = md5.ComputeHash(KEY);
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Encoding.UTF8.GetBytes($"{DateTime.Now.Millisecond}|{AccountID}|{rnd.Next() % 255}|{Name}|{rnd.Next() % 255}|{(int)Type}|{GetExpireTimeString()}|{Extra}");
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                    encrypt = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception)
            {
            }

            return encrypt;
        }

        public static Token FromString(string base64)
        {
            if (string.IsNullOrEmpty(base64))
            {
                return null;
            }

            string decrypt = null;
            try
            {
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                byte[] key = sha256.ComputeHash(KEY);
                byte[] iv = md5.ComputeHash(KEY);
                aes.Key = key;
                aes.IV = iv;

                byte[] dataByteArray = Convert.FromBase64String(base64);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(dataByteArray, 0, dataByteArray.Length);
                        cs.FlushFinalBlock();
                        decrypt = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(decrypt))
            {
                try
                {
                    Token t = new Token();
                    var lines = decrypt.Split('|');
                    if (lines.Length == 8)
                    {
                        t.AccountID = int.Parse(lines[1]);
                        t.Name = lines[3];
                        t.Type = (TokenType)int.Parse(lines[5]);
                        t.Extra = lines[7];

                        var y = FromCode(lines[6][0]) + 2020;
                        var M = FromCode(lines[6][1]);
                        var d = FromCode(lines[6][2]);
                        var h = FromCode(lines[6][3]);
                        var m = FromCode(lines[6][4]);
                        t.ExpireTime = new DateTime(y, M, d, h, m, 59);
                        return t;
                    }
                }
                catch (Exception)
                {
                }
            }

            return null;
        }

        public Token Renew()
        {
            this.ExpireTime = DateTime.Now.Add(ExpireInterval);
            return this;
        }

        public enum TokenType
        {
            Normal,
            Admin,
            Temp
        }
    }
}
