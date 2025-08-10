namespace npm.api.API.Controllers
{
    using NLog;
    using npm.api.DAO;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Http;
    using Web.Configs;

    [RoutePrefix("Home")]
    public partial class HomeController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected string FileRoot { get => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Config.Instance.CDNRoot); }

        protected string SettingFilePath
        {
            get => Path.Combine(FileRoot, "setting.json");
        }

        public HomeController()
        {
        }

        [HttpGet]
        public ResponseDTO GetHome()
        {
            using (var dao = GetDAO<BannerDAO>())
            using (var ldao = GetDAO<LessonDAO>(dao))
            {
                string setting = string.Empty;
                if (File.Exists(SettingFilePath))
                {
                    setting = File.ReadAllText(SettingFilePath);
                }

                return OK(new
                {
                    Banner = dao.GetAll("Active = 1").OrderBy(x => x.Sort).ThenByDescending(x => x.BannerID),
                    New = ldao.GetAll(" Active = 1 and IsDeleted = 0 ORDER BY CreateTime DESC LIMIT 9"),
                    Hot = ldao.GetAll(" Active = 1 and IsDeleted = 0 ORDER BY VisitCount DESC LIMIT 9"),
                    Setting = setting
                });
            }
        }
    }
}
