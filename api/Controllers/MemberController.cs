namespace npm.api.API.Controllers
{
    using Newtonsoft.Json;
    using NLog;
    using npm.api.API.Helper;
    using npm.api.API.Models;
    using npm.api.API.Service;
    using npm.api.DAO;
    using npm.api.DTO;
    using npm.api.Models.Dto;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Web;
    using Web.Configs;
    using Web.Core;
    using Web.Model;

    public class MemberController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static byte[] encryptKey = Encoding.UTF8.GetBytes("iPlalaceChannel");
        private static char[] authCodes = "0123456789".ToCharArray();

        public MemberController()
        {
        }

        [HttpPost]
        public ResponseDTO Login(LoginRequestModel request)
        {
            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    var pwd = EncodePassword(request.Password);
                    var user = dao.GetByEmail(request.Email);
                    if (user == null)
                    {
                        return Error(Status.LoginFail);
                    }
                    else if (EncodePassword(request.Password) != user.Password)
                    {
                        return Error(Status.LoginFail);
                    }

                    if (user.Status == MemberDTO.MemberStatus.Ban)
                    {
                        return Error(Status.Banned);
                    }

                    if (user.Status == MemberDTO.MemberStatus.Deleted)
                    {
                        return Error(Status.LoginFail);
                    }

                    var isExpiredPwd = false;
                    if (user.LastUpdatePasswordTime == null ||
                        (user.LastUpdatePasswordTime != null && user.LastUpdatePasswordTime.Value.AddMonths(3) < DateTime.Now))
                    {
                        isExpiredPwd = true;
                    }

                    user = dao.Get(user.MemberID);
                    var loginResponse = CreateLoginResponse(user, isExpiredPwd);
                    loginResponse.Status = isExpiredPwd ? Status.Expired : 0;

                    return loginResponse;
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        private ResponseDTO CreateLoginResponse(MemberDTO user, bool isExpiredPwd = false)
        {
            return OK(new MemberDto()
            {
                Name = user.Name,
                Type = (int)user.Type,
                School = user.School,
                Class = user.Class,
                Token = new Token()
                {
                    AccountID = user.MemberID,
                    Name = user.Name
                }.Generate(),
                IsExpiredPwd = isExpiredPwd
            });
        }

        [HttpPost]
        public ResponseDTO Register(MemberDTO request)
        {
            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    if (dao.GetByEmail(request.Email) != null)
                    {
                        return Error(Status.Duplicate);
                    }

                    request.Password = EncodePassword(request.Password);
                    request.Status = MemberDTO.MemberStatus.Normal;
                    request.Type = MemberDTO.MemberType.User;
                    request.LastUpdatePasswordTime = DateTime.Now;
                    request.MemberID = dao.Insert(request);

                    return CreateLoginResponse(request);
                }
                catch (Exception)
                {
                    return Error(Status.GeneralError);
                }
            }
        }

        public static string EncryptAES(string text)
        {
            var sourceBytes = System.Text.Encoding.UTF8.GetBytes(text);
            var aes = System.Security.Cryptography.Aes.Create();
            aes.Mode = System.Security.Cryptography.CipherMode.CBC;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            aes.Key = sha256.ComputeHash(encryptKey);
            aes.IV = md5.ComputeHash(encryptKey);
            var transform = aes.CreateEncryptor();
            return Convert.ToBase64String(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length));
        }

        public static string DecryptAES(string text)
        {
            var encryptBytes = System.Convert.FromBase64String(text);
            var aes = System.Security.Cryptography.Aes.Create();
            aes.Mode = System.Security.Cryptography.CipherMode.CBC;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            aes.Key = sha256.ComputeHash(encryptKey);
            aes.IV = md5.ComputeHash(encryptKey);
            var transform = aes.CreateDecryptor();
            return System.Text.Encoding.UTF8.GetString(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length));
        }

        [HttpGet]
        public ResponseDTO GetMe()
        {
            var t = Token;
            if (t == null)
            {
                return Error(Status.NotFound);
            }

            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    var user = dao.Get(t.AccountID);
                    if (user == null)
                    {
                        return Error(Status.LoginFail);
                    }

                    user.Password = string.Empty;
                    return OK(user);
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpPost]
        public ResponseDTO Update([FromBody] MemberDTO request)
        {
            var t = Token;
            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    var dto = dao.Get(t.AccountID);
                    dto.Email = request.Email;
                    if (!string.IsNullOrEmpty(request.Password))
                    {
                        dao.UpdatePassword(t.AccountID, EncodePassword(request.Password));
                    }

                    dto.Mobile = request.Mobile;
                    dto.Name = request.Name;

                    return OK(dao.Update(dto));
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpPost]
        public ResponseDTO UpdatePassword([FromBody] UpdatePasswordModel request)
        {
            var t = Token;
            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    var dto = dao.GetByEmail(request.Email);
                    if (dto.Password != EncodePassword(request.OldPassword))
                    {
                        return Error(Status.LoginFail);
                    }

                    dao.UpdatePassword(t.AccountID, EncodePassword(request.NewPassword));
                    return OK();
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpGet]
        public ResponseDTO GetOAuthUrl()
        {
            return OK($"{Config.Instance.OIDC.Host}/oidc/v1/azp?response_type=code&client_id={Config.Instance.OIDC.ClientID}&redirect_uri={HttpUtility.UrlEncode(Config.Instance.OIDC.RedirectURI)}&scope=educloudroles+openid2+eduinfo+email+profile+openid&state=2&nonce=1");
        }

        [HttpPost]
        public ResponseDTO OAuthLogin([FromBody] OAuthLoginRequestModel model)
        {
            string json = string.Empty;
            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    var key = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Config.Instance.OIDC.ClientID}:{Config.Instance.OIDC.ClientSecret}"));
                    var downloader = new Downloader();
                    downloader.ContentType = "application/x-www-form-urlencoded";

                    // downloader.Proxy = new System.Net.WebProxy("127.0.0.1", 8888);
                    json = downloader.DownloadString($"{Config.Instance.OIDC.Host}/oidc/v1/token", null, $"grant_type=authorization_code&code={model.Code}&redirect_uri={HttpUtility.UrlEncode(Config.Instance.OIDC.RedirectURI)}", null, new Dictionary<string, string>
                    {
                        { "authorization", $"Basic {key}" }
                    });
                    var token = JsonConvert.DeserializeObject<TokenResult>(json);
                    var auth = new Dictionary<string, string>
                    {
                        { "authorization", $"Bearer {token.AccessToken}" }
                    };
                    json = downloader.DownloadString($"{Config.Instance.OIDC.Host}/moeresource/api/v1/oidc/userinfo", null, null, null, auth);

                    Action logout = () =>
                    {
                        try
                        {
                            json = downloader.DownloadString($"{Config.Instance.OIDC.Host}oidc/v1/userlogout?Id_token_hint={HttpUtility.UrlEncode(token.IdToken)}&post_logout_redirect_url={HttpUtility.UrlEncode("https://ipalace.npm.edu.tw/")}");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    };
                    var info = JsonConvert.DeserializeObject<UserInfoResult>(json);
                    var member = dao.GetByEmail(info.Email);
                    if (member != null)
                    {
                        if (member.OpenID != info.Sub.ToString())
                        {
                            return Error(Status.Duplicate);
                        }

                        logout();

                        return CreateLoginResponse(member);
                    }

                    json = downloader.DownloadString($"{Config.Instance.OIDC.Host}/moeresource/api/v1/oidc/eduinfo", null, null, null, auth);
                    var eduinfo = JsonConvert.DeserializeObject<EduInfoResult>(json);
                    json = downloader.DownloadString($"{Config.Instance.OIDC.OpenAPIHost}/moeresource/api/v2/eduoid/all/eduid/{eduinfo.Schoolid}");
                    var school = JsonConvert.DeserializeObject<IEnumerable<SchoolResult>>(json).FirstOrDefault();

                    var request = new MemberDTO()
                    {
                        Name = info.Name,
                        Email = info.Email,
                        Status = MemberDTO.MemberStatus.Normal,
                        OpenID = info.Sub.ToString(),
                        City = !string.IsNullOrEmpty(school.County) ? school.County.Split(']')[1] : null,
                        School = school.Schoolname,
                        Class = eduinfo.Classinfo.FirstOrDefault()?.Classtitle
                    };

                    if (eduinfo.Titles.Any(x => x.Titles.Contains("學生")))
                    {
                        request.Type = MemberDTO.MemberType.Student;
                    }
                    else
                    {
                        request.Type = MemberDTO.MemberType.Teacher;
                    }

                    request.Status = MemberDTO.MemberStatus.Normal;
                    request.MemberID = dao.Insert(request);

                    logout();

                    return CreateLoginResponse(request);
                }
                catch (Exception ex)
                {
                    return Error(Status.GeneralError, ex.Message + json);
                }
            }
        }

        [HttpGet]
        public ResponseDTO GetCaptcha()
        {
            CaptchaService service = new CaptchaService();
            var result = service.GenerateCaptchaImage(140, 43, service.GenerateCaptchaCode());
            return OK(new
            {
                Secret = EncryptAES($"{result.code},{DateTime.Now.AddMinutes(5).Ticks}"),
                Image = "data:image/jpeg;base64," + result.image
            });
        }

        [HttpPost]
        public async Task<ResponseDTO> GetAudioCaptchaAsync([FromBody] AudioCaptchaRequestDto audioCaptchaRequestDto)
        {
            var authCode = string.Empty;

            if (!CheckSecret(audioCaptchaRequestDto.Secret, ref authCode))
            {
                // return Error(Status.WaitVerify);
            }

            var audioPath = await AudioHelper.GetAudioAsync(authCode, "audio");

            return OK(new
            {
                AudioUrl = audioPath
            });
        }

        [HttpPost]
        public ResponseDTO SendForgetPassword([FromBody] SendForgetMailModel model)
        {
            if (!CheckAuthCode(model.AuthCode, model.Secret))
            {
                return Error(Status.WaitVerify);
            }

            using (var dao = GetDAO<MemberDAO>())
            {
                var member = dao.GetByEmail(model.Email);
                if (member == null)
                {
                    return Error(Status.NotFound);
                }

                if (!string.IsNullOrEmpty(member.OpenID))
                {
                    return Error(Status.Forbidden);
                }

                var service = new EmailService();
                service.Send(member.Email, "更新密碼通知信", "password", new
                {
                    member,
                    url = "https://ipalace.npm.edu.tw/mail/forget?t=" + HttpUtility.UrlEncode(new Token()
                    {
                        AccountID = member.MemberID,
                        ExpireTime = DateTime.Now.AddHours(1),
                        Type = Token.TokenType.Temp
                    }.Generate())
                });
                return OK();
            }
        }

        [HttpPost]
        public ResponseDTO ResetPassword([FromBody] MemberDTO request)
        {
            var t = Token;
            using (var dao = GetDAO<MemberDAO>())
            {
                try
                {
                    dao.UpdatePassword(t.AccountID, EncodePassword(request.Password));
                    return OK();
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        private bool CheckAuthCode(string authCode, string secret)
        {
            try
            {
                var data = DecryptAES(secret).Split(',');
                if (data[0] != authCode)
                {
                    return false;
                }

                if (DateTime.Now.Ticks > long.Parse(data[1]))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool CheckSecret(string secret, ref string authCode)
        {
            try
            {
                var decryptResult = DecryptAES(secret);
                if (decryptResult == null || string.IsNullOrEmpty(decryptResult))
                {
                    return false;
                }

                var data = DecryptAES(secret).Split(',');
                if (string.IsNullOrEmpty(data[0]))
                {
                    return false;
                }

                if (DateTime.Now.Ticks > long.Parse(data[1]))
                {
                    return false;
                }

                authCode = data[0];

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string EncodePassword(string password)
        {
            MD5 md5 = MD5.Create();
            return string.Join("", md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
        }

        public partial class TokenResult
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("scope")]
            public string Scope { get; set; }

            [JsonProperty("id_token")]
            public string IdToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public long ExpiresIn { get; set; }
        }
    }

    public partial class UserInfoResult
    {
        [JsonProperty("sub")]
        public Guid Sub { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("preferred_username")]
        public string PreferredUsername { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }

    public partial class EduInfoResult
    {
        [JsonProperty("schoolid")]
        public string Schoolid { get; set; }

        [JsonProperty("sub")]
        public Guid Sub { get; set; }

        [JsonProperty("titles")]
        public List<Title> Titles { get; set; }

        [JsonProperty("classinfo")]
        public List<Classinfo> Classinfo { get; set; }
    }

    public partial class Classinfo
    {
        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("semester")]
        public string Semester { get; set; }

        [JsonProperty("schoolid")]
        public string Schoolid { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        [JsonProperty("classno")]
        public string Classno { get; set; }

        [JsonProperty("classtitle")]
        public string Classtitle { get; set; }
    }

    public partial class Title
    {
        [JsonProperty("schoolid")]
        public string Schoolid { get; set; }

        [JsonProperty("titles")]
        public List<string> Titles { get; set; }
    }

    public partial class SchoolResult
    {
        [JsonProperty("eduid")]
        public string Eduid { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("schoolname")]
        public string Schoolname { get; set; }

        [JsonProperty("county")]
        public string County { get; set; }

        [JsonProperty("oid")]
        public string Oid { get; set; }

        [JsonProperty("tel")]
        public string Tel { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("coordinates")]
        public string Coordinates { get; set; }

        [JsonProperty("schoolsystem")]
        public List<string> Schoolsystem { get; set; }
    }

    public class CaptchaService
    {
        private const string Letters = "0123456789";

        public string GenerateCaptchaCode()
        {
            Random rand = new Random();
            int maxRand = Letters.Length - 1;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                int index = rand.Next(maxRand);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        }

        public (string code, string image) GenerateCaptchaImage(int width, int height, string captchaCode)
        {
            using (Bitmap baseMap = new Bitmap(width, height))
            using (Graphics graph = Graphics.FromImage(baseMap))
            {
                Random rand = new Random();

                graph.Clear(GetRandomLightColor());

                DrawCaptchaCode();
                DrawDisorderLine();
                AdjustRippleEffect();

                MemoryStream ms = new MemoryStream();

                baseMap.Save(ms, ImageFormat.Jpeg);

                return (captchaCode, Convert.ToBase64String(ms.ToArray()));

                int GetFontSize(int imageWidth, int captchCodeCount)
                {
                    var averageSize = imageWidth / captchCodeCount;

                    return Convert.ToInt32(averageSize);
                }

                Color GetRandomDeepColor()
                {
                    int redlow = 160, greenLow = 100, blueLow = 160;
                    return Color.FromArgb(rand.Next(redlow), rand.Next(greenLow), rand.Next(blueLow));
                }

                Color GetRandomLightColor()
                {
                    int low = 180, high = 255;

                    int nRend = (rand.Next(high) % (high - low)) + low;
                    int nGreen = (rand.Next(high) % (high - low)) + low;
                    int nBlue = (rand.Next(high) % (high - low)) + low;

                    return Color.FromArgb(nRend, nGreen, nBlue);
                }

                void DrawCaptchaCode()
                {
                    SolidBrush fontBrush = new SolidBrush(Color.Black);
                    int fontSize = GetFontSize(width, captchaCode.Length);
                    Font font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                    for (int i = 0; i < captchaCode.Length; i++)
                    {
                        fontBrush.Color = GetRandomDeepColor();

                        int shiftPx = fontSize / 6;

                        float x = (i * fontSize) + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);
                        if (x < 0)
                        {
                            x = 0;
                        }

                        int maxY = height - fontSize;

                        if (maxY < 0)
                        {
                            maxY = 0;
                        }

                        float y = rand.Next(0, maxY);

                        graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
                    }
                }

                void DrawDisorderLine()
                {
                    Pen linePen = new Pen(new SolidBrush(Color.Black), 3);
                    for (int i = 0; i < rand.Next(3, 5); i++)
                    {
                        linePen.Color = GetRandomDeepColor();

                        Point startPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                        Point endPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                        graph.DrawLine(linePen, startPoint, endPoint);

                        // Point bezierPoint1 = new Point(rand.Next(0, width), rand.Next(0, height));
                        // Point bezierPoint2 = new Point(rand.Next(0, width), rand.Next(0, height));
                        // graph.DrawBezier(linePen, startPoint, bezierPoint1, bezierPoint2, endPoint);
                    }
                }

                void AdjustRippleEffect()
                {
                    short nWave = 6;
                    int nWidth = baseMap.Width;
                    int nHeight = baseMap.Height;

                    Point[,] pt = new Point[nWidth, nHeight];

                    for (int x = 0; x < nWidth; ++x)
                    {
                        for (int y = 0; y < nHeight; ++y)
                        {
                            var xo = nWave * Math.Sin(2.0 * 3.1415 * y / 128.0);
                            var yo = nWave * Math.Cos(2.0 * 3.1415 * x / 128.0);

                            var newX = x + xo;
                            var newY = y + yo;

                            if (newX > 0 && newX < nWidth)
                            {
                                pt[x, y].X = (int)newX;
                            }
                            else
                            {
                                pt[x, y].X = 0;
                            }

                            if (newY > 0 && newY < nHeight)
                            {
                                pt[x, y].Y = (int)newY;
                            }
                            else
                            {
                                pt[x, y].Y = 0;
                            }
                        }
                    }

                    Bitmap bSrc = (Bitmap)baseMap.Clone();

                    BitmapData bitmapData = baseMap.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                    int scanline = bitmapData.Stride;

                    IntPtr scan0 = bitmapData.Scan0;
                    IntPtr srcScan0 = bmSrc.Scan0;

                    unsafe
                    {
                        byte* p = (byte*)(void*)scan0;
                        byte* pSrc = (byte*)(void*)srcScan0;

                        int nOffset = bitmapData.Stride - (baseMap.Width * 3);

                        for (int y = 0; y < nHeight; ++y)
                        {
                            for (int x = 0; x < nWidth; ++x)
                            {
                                var xOffset = pt[x, y].X;
                                var yOffset = pt[x, y].Y;

                                if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                                {
                                    if (pSrc != null)
                                    {
                                        p[0] = pSrc[(yOffset * scanline) + (xOffset * 3)];
                                        p[1] = pSrc[(yOffset * scanline) + (xOffset * 3) + 1];
                                        p[2] = pSrc[(yOffset * scanline) + (xOffset * 3) + 2];
                                    }
                                }

                                p += 3;
                            }

                            p += nOffset;
                        }
                    }

                    baseMap.UnlockBits(bitmapData);
                    bSrc.UnlockBits(bmSrc);
                    bSrc.Dispose();
                }
            }
        }
    }
}