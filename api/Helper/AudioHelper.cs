namespace npm.api.API.Helper
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Speech.AudioFormat;
    using System.Speech.Synthesis;
    using System.Text;
    using System.Threading.Tasks;
    using Web.Configs;

    public class AudioHelper
    {
        public static string FileRoot { get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.Instance.CDNRoot); }

        public static async Task<string> GetAudioAsync(string input, params string[] directory)
        {
            var root = Path.Combine(new string[] { FileRoot }.Concat(directory).ToArray());
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var fileName = "AudioCaptcha_" + Md5(input) + ".wav";
            var path = Path.Combine(root, fileName);

            try
            {
                var task = Task.Run(async () =>
                {
                    using (SpeechSynthesizer ss = new SpeechSynthesizer())
                    {
                        await SaveAudioToMp3Async(input, path, ss);
                    }
                });
                Task.WaitAny(task);
            }
            catch (AggregateException ae)
            {
                var sb = new StringBuilder();
                foreach (var item in ae.InnerExceptions)
                {
                    var t = item.GetType();
                    sb.AppendLine(t.Namespace + "." + t.Name + " " + item.Message);
                }
            }

            return Path.Combine(directory.Concat(new string[] { fileName }).ToArray()).Replace("\\", "/");
        }

        private static async Task SaveAudioToMp3Async(string inputText, string filePath, SpeechSynthesizer ss)
        {
            ss.Volume = 100;
            ss.SetOutputToDefaultAudioDevice();

            // Configure the audio output.
            ss.SetOutputToWaveFile(@filePath,
              new SpeechAudioFormatInfo(32000, AudioBitsPerSample.Sixteen, AudioChannel.Mono));

            // Create a SoundPlayer instance to play output audio file.
            System.Media.SoundPlayer m_SoundPlayer =
              new System.Media.SoundPlayer(@filePath);

            // Build a prompt.
            PromptBuilder builder = new PromptBuilder(new System.Globalization.CultureInfo("zh-tw"));
            var splitInputTexts = inputText.Select(digit => int.Parse(digit.ToString()));
            builder.StartParagraph();
            foreach (var splitInputText in splitInputTexts.ToArray())
            {
                builder.StartSentence();
                builder.AppendText(splitInputText.ToString());
                builder.EndSentence();
            }

            builder.EndParagraph();

            // Speak the prompt.
            ss.Speak(builder);
            m_SoundPlayer.Play();
        }

        /// <summary>
        /// MD5加密，主要根據輸入的文字來生成唯一的音頻文件名
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string Md5(string value)
        {
            var result = string.Empty;
            if (string.IsNullOrEmpty(value))
            {
                return result;
            }

            using (var md5 = MD5.Create())
            {
                result = GetMd5Hash(md5, value);
            }

            return result;
        }

        /// <summary>
        /// MD5生成
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}