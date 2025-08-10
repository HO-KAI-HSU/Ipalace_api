namespace npm.api.API.Controllers
{
    using npm.api.API.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Http;
    using Web.Configs;

    public partial class BackendController : BaseController
    {
        protected string FileRoot { get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.Instance.CDNRoot); }

        protected string FilePath
        {
            get => Path.Combine(FileRoot, "style.css");
        }

        protected string MobileFilePath
        {
            get => Path.Combine(FileRoot, "rwd.css");
        }

        protected string JSFilePath
        {
            get => Path.Combine(FileRoot, "custom.js");
        }

        protected string SettingFilePath
        {
            get => Path.Combine(FileRoot, "setting.json");
        }

        [HttpPost]
        public ResponseDTO UpdateCss([FromBody] SaveCssModel model)
        {
            System.IO.File.WriteAllText(FilePath, model.PC);
            System.IO.File.WriteAllText(MobileFilePath, model.Mobile);
            System.IO.File.WriteAllText(JSFilePath, model.Custom);
            return OK();
        }

        [HttpGet]
        public ResponseDTO GetCss()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>
            {
                { "pc", "" }, { "mobile", "" }, { "custom", "" }
            };

            if (File.Exists(FilePath))
            {
                ret["pc"] = File.ReadAllText(FilePath);
            }

            if (File.Exists(MobileFilePath))
            {
                ret["mobile"] = File.ReadAllText(MobileFilePath);
            }

            if (File.Exists(JSFilePath))
            {
                ret["custom"] = File.ReadAllText(JSFilePath);
            }

            return OK(ret);
        }

        [HttpGet]
        public ResponseDTO GetSetting()
        {
            if (System.IO.File.Exists(SettingFilePath))
            {
                return OK(System.IO.File.ReadAllText(SettingFilePath));
            }

            return OK();
        }

        [HttpPost]
        public ResponseDTO UpdateSetting([FromBody] SaveSettingModel model)
        {
            System.IO.File.WriteAllText(SettingFilePath, model.Content);
            return OK();
        }
    }
}
