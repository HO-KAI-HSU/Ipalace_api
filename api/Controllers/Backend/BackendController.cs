namespace npm.api.API.Controllers
{
    using NLog;
    using System.Web.Http;
    using Web.Filter;

    [RoutePrefix("Backend")]
    [AuthFilter]
    public partial class BackendController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public BackendController()
        {
        }
    }
}
