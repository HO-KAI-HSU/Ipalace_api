namespace npm.api.API.Controllers
{
    using Dapper;
    using npm.api.API.Helper;
    using npm.api.API.Models.Backend;
    using npm.api.API.Models.Backend.Dto;
    using npm.api.API.Service;
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    public partial class BackendController : BaseController
    {
        [HttpGet]
        public ResponseDTO GetTeachPlanCategories()
        {
            using (var dao = GetDAO<TeachPlanCategoryDAO>())
            {
                return OK(dao.GetAll());
            }
        }

        [HttpGet]
        public ResponseDTO GetTeachPlans()
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            {
                return OK(dao.GetAll("IsDeleted = @isDeleted", new { isDeleted = false }).OrderByDescending(x => x.CreateTime));
            }
        }

        [HttpGet]
        public ResponseDTO GetTeachPlan(int id)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            using (var tpfdao = GetDAO<TeachPlanFileDAO>(dao))
            {
                var dto = dao.Get(id);
                dto.TeachPlanFiles = tpfdao
                    .GetAll("TeachPlanID = @id and Active = @active", new { id, active = true })
                    .ToList();
                return OK(dto);
            }
        }

        [HttpPost]
        public ResponseDTO CreateTeachPlan(TeachPlanDTO model)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            using (var tpfdao = GetDAO<TeachPlanFileDAO>(dao))
            {
                var teachPlanFilesDto = tpfdao.GetAll("Url IN @urls", new
                {
                    urls = model.TeachPlanFiles
                    .Select(x => x.Url)
                    .ToArray()
                }).ToList();

                var trans = dao.BeginTransaction();
                var id = dao.Insert(model);
                teachPlanFilesDto.ForEach(x =>
                {
                    x.TeachPlanID = id;
                    x.Name = model.TeachPlanFiles
                        .FirstOrDefault(y => y.Url.Equals(x.Url)).Name;
                    x.UpdateTime = DateTime.Now;
                    x.Active = true;
                    tpfdao.Update(x);
                });
                trans.Commit();
                return OK();
            }
        }

        [HttpPost]
        public ResponseDTO UpdateTeachPlan(TeachPlanDTO model)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            using (var tpfdao = GetDAO<TeachPlanFileDAO>(dao))
            {
                var trans = dao.BeginTransaction();

                // We need to update the active status of the files(Dont use)
                var dbTeachPlanFilesDto = tpfdao
                    .GetAll("TeachPlanID = @id", new { id = model.TeachPlanID })
                    .ToList();

                dbTeachPlanFilesDto.ForEach(x =>
                {
                    if (!model.TeachPlanFiles.Any(y => y.Url.Equals(x.Url)))
                    {
                        x.TeachPlanID = 0;
                        x.Active = false;
                        x.UpdateTime = DateTime.Now;
                        tpfdao.Update(x);
                    }
                });

                // We need to update the active status of the files(Add)
                var teachPlanFilesDto = tpfdao.GetAll("TeachPlanID = 0 and Url IN @urls", new
                {
                    urls = model.TeachPlanFiles
                    .Select(x => x.Url)
                    .ToArray()
                }).ToList();

                teachPlanFilesDto.ForEach(x =>
                {
                    x.TeachPlanID = model.TeachPlanID;
                    x.Name = model.TeachPlanFiles
                        .FirstOrDefault(y => y.Url.Equals(x.Url)).Name;
                    x.UpdateTime = DateTime.Now;
                    x.Active = true;
                    tpfdao.Update(x);
                });

                var dto = dao.Get(model.TeachPlanID);
                dto.TeachPlanCategoryID = model.TeachPlanCategoryID;
                dto.Name = model.Name;
                dto.Teacher = model.Teacher;
                dto.School = model.School;
                dto.Domain = model.Domain;
                dto.ExternalLink = model.ExternalLink;
                dto.VideoName = model.VideoName;
                dto.Goal = model.Goal;
                dto.TeachPlanUrl = model.TeachPlanUrl;
                dto.TeachPlanFileName = model.TeachPlanFileName;
                dto.LearnSheetUrl = model.LearnSheetUrl;
                dto.LearnSheetFileName = model.LearnSheetFileName;
                dto.VideoUrl = model.VideoUrl;
                dto.VideoFileName = model.VideoFileName;
                dto.LessonID = model.LessonID;

                if (model.LessonID == null)
                {
                    dao.GetConnection().Execute("UPDATE TeachPlan SET LessonID = null WHERE TeachPlanID = @TeachPlanID", model);
                }

                dao.Update(dto);
                trans.Commit();
                return OK();
            }
        }

        [HttpDelete]
        public ResponseDTO DeleteTeachPlan(int id)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            {
                var model = dao.Get(id);
                model.IsDeleted = true;
                return OK(dao.Update(model));
            }
        }

        [HttpPost]
        public async Task<ResponseDTO> UploadTeachPlanFile()
        {
            var uploadSrvice = new UploadService();
            var result = await uploadSrvice.Upload(this.Request, "techplan/plan");

            using (var dao = GetDAO<TeachPlanFileDAO>())
            {
                var model = new TeachPlanFileDTO()
                {
                    Url = result.Url,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                dao.Insert(model);
            }

            return OK(result);
        }

        [HttpPost]
        public async Task<ResponseDTO> UploadLearnSheetFile()
        {
            var uploadSrvice = new UploadService();
            var result = await uploadSrvice.Upload(this.Request, "techplan/sheet");
            return OK(result);
        }

        [HttpPost]
        public async Task<ResponseDTO> UploadTeachPlanVideo()
        {
            var uploadSrvice = new UploadService();
            var result = await uploadSrvice.Upload(this.Request, "techplan/video");
            return OK(result);
        }

        [HttpPost]
        public ResponseDTO UpdateTeachPlanPublishStatus(int id)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            {
                var model = dao.Get(id);
                model.Active = !model.Active;
                return OK(dao.Update(model));
            }
        }

        [HttpGet]
        public ResponseDTO GetTeachPlanStatisticList()
        {
            var queryString = Request
                .GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => x.Value);

            var orderBy = "TotalCount";

            if (queryString.TryGetValue("startDate", out string startDate))
            {
            }

            if (queryString.TryGetValue("endDate", out string endDate))
            {
            }

            if (queryString.TryGetValue("sortBy", out string sortBy))
            {
                if (!string.IsNullOrEmpty(sortBy) && sortBy.Equals("Count"))
                {
                    orderBy = "TotalCount";
                }
            }

            if (queryString.TryGetValue("limit", out string limit))
            {
            }

            using (var dao = GetDAO<TeachPlanDAO>())
            using (var vtpsdao = GetDAO<VTeachPlanStatisticsDAO>(dao))
            {
                var teachPlans = dao.GetAll("IsDeleted = @isDeleted", new { isDeleted = false });
                var statistics = (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) ?
                    vtpsdao.GetAll()
                    .GroupBy(x => new { x.TeachPlanID })
                    .Select(x => new TeachPlanStatisticModel
                    {
                        TeachPlanID = x.Key.TeachPlanID,
                        TotalCount = x
                            .Where(y => y.Type == 2)
                            .Sum(y => y.TotalCount)
                    }) :
                    vtpsdao.GetAll("Date >= @start AND Date <= @end", new { start = startDate, end = endDate })
                    .GroupBy(x => new { x.TeachPlanID })
                    .Select(x => new TeachPlanStatisticModel
                    {
                        TeachPlanID = x.Key.TeachPlanID,
                        TotalCount = x
                            .Where(y => y.Type == 2)
                            .Sum(y => y.TotalCount)
                    });

                var teachPlanStatistics = teachPlans
                    .GroupJoin(statistics,
                        x => x.TeachPlanID,
                        y => y.TeachPlanID,
                        (x, y) => new { TeachPlan = x, TeachPlanStatistic = y })
                    .SelectMany(x => x.TeachPlanStatistic.DefaultIfEmpty(), (x, y) => new TeachPlanStatisticDto()
                    {
                        TeachPlanID = x.TeachPlan.TeachPlanID,
                        TeachPlanName = x.TeachPlan.Name,
                        TotalCount = y == null ? 0 : y.TotalCount,
                    })
                    .OrderByDescending(x => CommonHelper.GetPropertyValue(x, orderBy));

                if (!string.IsNullOrEmpty(limit))
                {
                    teachPlanStatistics.Take(Convert.ToInt16(limit));
                }

                return OK(teachPlanStatistics);
            }
        }
    }
}