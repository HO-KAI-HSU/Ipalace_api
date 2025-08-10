namespace npm.api.API.Controllers
{
    using npm.api.API.Models.Backend;
    using npm.api.API.Service;
    using npm.api.DAO;
    using npm.api.DTO;
    using System.Threading.Tasks;
    using System.Web.Http;

    public partial class BackendController : BaseController
    {
        [HttpPost]
        public ResponseDTO UpdateBannerSort([FromBody] UpdateSortModel<BannerDTO> model)
        {
            using (var dao = GetDAO<BannerDAO>())
            {
                return OK(dao.UpdateSort(model.Data));
            }
        }

        [HttpGet]
        public ResponseDTO GetBanners()
        {
            using (var dao = GetDAO<BannerDAO>())
            {
                return OK(dao.GetAll());
            }
        }

        [HttpGet]
        public ResponseDTO GetBanner(int id)
        {
            using (var dao = GetDAO<BannerDAO>())
            {
                return OK(dao.Get(id));
            }
        }

        [HttpPost]
        public ResponseDTO CreateBanner(BannerDTO model)
        {
            using (var dao = GetDAO<BannerDAO>())
            {
                return OK(dao.Insert(model));
            }
        }

        [HttpPost]
        public ResponseDTO UpdateBanner(BannerDTO model)
        {
            using (var dao = GetDAO<BannerDAO>())
            {
                return OK(dao.Update(model));
            }
        }

        [HttpPost]
        public async Task<ResponseDTO> UploadBannerImage()
        {
            var uploadSrvice = new UploadService();
            var result = await uploadSrvice.Upload(this.Request, "banner");
            return OK(result);
        }
    }
}
