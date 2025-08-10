namespace npm.api.API.Controllers
{
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
        public ResponseDTO GetLessonCategories()
        {
            using (var dao = GetDAO<LessonCategoryDAO>())
            {
                return OK(dao.GetAll());
            }
        }

        [HttpGet]
        public ResponseDTO GetLessonCategory(int id)
        {
            using (var dao = GetDAO<LessonCategoryDAO>())
            {
                return OK(dao.Get(id));
            }
        }

        [HttpPost]
        public ResponseDTO CreateLessonCategory(LessonCategoryDTO model)
        {
            using (var dao = GetDAO<LessonCategoryDAO>())
            {
                return OK(dao.Insert(model));
            }
        }

        [HttpGet]
        public ResponseDTO GetLessons()
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetAll("IsDeleted = @isDeleted", new { isDeleted = false }).OrderByDescending(x => x.CreateTime));
            }
        }

        [HttpGet]
        public ResponseDTO GetLesson(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var hwdao = GetDAO<HomeWorkDAO>(dao))
            using (var lcdao = GetDAO<LessonChapterDAO>(dao))
            {
                return OK(new
                {
                    Lesson = dao.Get(id),
                    HomeWork = hwdao.Get(id),
                    Chapters = lcdao.GetAll($"LessonID = {id}")
                });
            }
        }

        [HttpPost]
        public ResponseDTO CreateLesson(LessonModel model)
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var hwdao = GetDAO<HomeWorkDAO>(dao))
            using (var lcdao = GetDAO<LessonChapterDAO>(dao))
            {
                var trans = dao.BeginTransaction();
                var id = dao.Insert(model.Lesson, trans);
                model.HomeWork.LessonID = id;
                hwdao.Insert(model.HomeWork, trans);
                foreach (var c in model.Chapters)
                {
                    c.LessonID = id;
                    lcdao.Insert(c, trans);
                }

                trans.Commit();
                return OK();
            }
        }

        [HttpPost]
        public ResponseDTO UpdateLesson(LessonModel model)
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var hwdao = GetDAO<HomeWorkDAO>(dao))
            using (var lcdao = GetDAO<LessonChapterDAO>(dao))
            {
                var trans = dao.BeginTransaction();
                dao.Update(model.Lesson, trans);
                model.HomeWork.LessonID = model.Lesson.LessonID;
                hwdao.Update(model.HomeWork, trans);
                lcdao.Delete("LessonID = " + model.Lesson.LessonID);

                foreach (var c in model.Chapters)
                {
                    c.LessonID = model.Lesson.LessonID;
                    lcdao.Insert(c, trans);
                }

                trans.Commit();
                return OK();
            }
        }

        [HttpDelete]
        public ResponseDTO DeleteLesson(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                var model = dao.Get(id);
                model.IsDeleted = true;
                return OK(dao.Update(model));
            }
        }

        [HttpPost]
        public async Task<ResponseDTO> UploadLessonImageFile()
        {
            var uploadSrvice = new UploadService();
            var result = await uploadSrvice.Upload(this.Request, "lesson/image");
            return OK(result);
        }

        [HttpPost]
        public async Task<ResponseDTO> UploadLessonVideoFile()
        {
            var uploadSrvice = new UploadService();
            var result = await uploadSrvice.Upload(this.Request, "lesson/video");
            return OK(result);
        }

        [HttpPost]
        public ResponseDTO UpdateLessonPublishStatus(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                var model = dao.Get(id);
                model.Active = !model.Active;
                return OK(dao.Update(model));
            }
        }

        [HttpGet]
        public ResponseDTO GetLessonStatisticList()
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
                if (!string.IsNullOrEmpty(sortBy) && sortBy == "Count")
                {
                    orderBy = "TotalCount";
                }

                if (!string.IsNullOrEmpty(sortBy) && sortBy == "Time")
                {
                    orderBy = "TotalTime";
                }
            }

            if (queryString.TryGetValue("limit", out string limit))
            {
            }

            using (var dao = GetDAO<LessonDAO>())
            using (var vlsdao = GetDAO<VLessonStatisticsDAO>(dao))
            using (var vlfsdao = GetDAO<VLessonFinishedStatisticsDAO>(dao))
            {
                var lessons = dao.GetAll("IsDeleted = @isDeleted", new { isDeleted = false });
                var statistics = (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) ?
                    vlsdao.GetAll()
                    .GroupBy(x => new { x.LessonID })
                    .Select(x => new LessonStatisticModel
                    {
                        LessonID = x.Key.LessonID,
                        TotalCount = x
                          .Where(y => y.Type == 1)
                          .Sum(y => y.TotalCount),
                        TotalTime = x
                          .Where(y => y.Type == 2)
                          .Sum(y => y.TotalTime)
                    }) :
                vlsdao.GetAll("Date >= @Start AND Date <= @End", new { Start = startDate, End = endDate })
                    .GroupBy(x => new { x.LessonID })
                    .Select(x => new LessonStatisticModel
                    {
                        LessonID = x.Key.LessonID,
                        TotalCount = x
                            .Where(y => y.Type == 1)
                            .Sum(y => y.TotalCount),
                        TotalTime = x
                            .Where(y => y.Type == 2)
                            .Sum(y => y.TotalTime)
                    });

                var vlfsStatistics = (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) ?
                    vlfsdao.GetAll()
                    .GroupBy(x => new { x.LessonID })
                    .Select(x => new LessonStatisticModel
                    {
                        LessonID = x.Key.LessonID,
                        DoneCount = x.Sum(y => y.TotalCount)
                    }) :
                    vlfsdao.GetAll("Date >= @Start AND Date <= @End", new { Start = startDate, End = endDate })
                    .GroupBy(x => new { x.LessonID })
                    .Select(x => new LessonStatisticModel
                    {
                        LessonID = x.Key.LessonID,
                        DoneCount = x.Sum(y => y.TotalCount)
                    });

                var lessonStatistics = lessons
                    .GroupJoin(vlfsStatistics,
                        x => x.LessonID,
                        y => y.LessonID,
                        (x, y) => new { Lesson = x, LessonFinishStatistic = y.FirstOrDefault() })
                    .GroupJoin(statistics,
                        x => x.Lesson.LessonID,
                        y => y.LessonID,
                        (x, y) => new { x.Lesson, x.LessonFinishStatistic, LessonStatistic = y.FirstOrDefault() })
                    .Select(x => new LessonStatisticDto()
                    {
                        LessonID = x.Lesson.LessonID,
                        LessonName = x.Lesson.LessonName,
                        TotalCount = x.LessonStatistic == null ? 0 : x.LessonStatistic.TotalCount,
                        TotalTime = (float)(x.LessonStatistic == null ? 0 : Math.Ceiling(x.LessonStatistic.TotalTime)),
                        DoneCount = x.LessonFinishStatistic == null ? 0 : x.LessonFinishStatistic.DoneCount,
                    })
                    .OrderByDescending(x => CommonHelper.GetPropertyValue(x, orderBy));

                if (!string.IsNullOrEmpty(limit))
                {
                    lessonStatistics.Take(Convert.ToInt16(limit));
                }

                return OK(lessonStatistics);
            }
        }
    }
}