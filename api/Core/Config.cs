namespace Web.Configs
{
    using Newtonsoft.Json;
    using System;
    using System.IO;

    public class Config
    {
        private const string FileName = "appsettings.json";

        private static ConfigData _Instance;

        public static ConfigData Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName)));
                }

                return _Instance;
            }
        }

        public static void Save()
        {
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName), JsonConvert.SerializeObject(_Instance, Formatting.Indented));
        }

        public class ConfigData
        {
            [JsonProperty("Database")]
            public Database Database { get; set; }

            [JsonProperty("Backend")]
            public Backend Backend { get; set; }

            [JsonProperty("CDNRoot")]
            public string CDNRoot { get; set; }

            [JsonProperty("OIDC")]
            public OIDC OIDC { get; set; }

            [JsonProperty("FrontendUrl")]
            public string FrontendUrl { get; set; }

            [JsonProperty("CDNUrl")]
            public string CDNUrl { get; set; }

            [JsonProperty("Email")]
            public SMTPConfig Email { get; set; }
        }

        public class Database
        {
            [JsonProperty("ConnectionString")]
            public string ConnectionString { get; set; }
        }

        public class Backend
        {
            [JsonProperty("UserName")]
            public string UserName { get; set; }

            [JsonProperty("Password")]
            public string Password { get; set; }

            [JsonProperty("LastUpdateTime")]
            public DateTime? LastUpdateTime { get; set; } = null;
        }

        public class OIDC
        {
            [JsonProperty("Host")]
            public string Host { get; set; }

            [JsonProperty("OpenAPIHost")]
            public string OpenAPIHost { get; set; }

            [JsonProperty("ClientID")]
            public string ClientID { get; set; }

            [JsonProperty("ClientSecret")]
            public string ClientSecret { get; set; }

            [JsonProperty("RedirectURI")]
            public string RedirectURI { get; set; }
        }

        public class SMTPConfig
        {
            public string UserName { get; set; }

            public string Password { get; set; }

            public string Host { get; set; }

            public int Port { get; set; }

            public string SenderAddress { get; set; }
        }
    }
}
