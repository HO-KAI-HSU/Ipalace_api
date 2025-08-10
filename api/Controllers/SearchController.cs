namespace npm.api.API.Controllers
{
    using NLog;
    using npm.api.DAO;
    using npm.api.DTO;
    using npm.api.Enums;
    using npm.api.Models.Dto;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;

    [RoutePrefix("Search")]
    public class SearchController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SearchController()
        {
        }

        [HttpGet]
        public ResponseDTO KeywordSearch(string id)
        {
            var searchs = new List<SearchDto>();
            var lessons = GetLessonSearchResult(id);
            var teachPlans = GetTeachPlanSearchResult(id);

            foreach (var lesson in lessons)
            {
                searchs.Add(new SearchDto()
                {
                    Title = lesson.LessonName,
                    Description = lesson.Brief,
                    Type = lesson.LessonCategoryID,
                    ImageUrl = lesson.ImageUrl,
                    LinkUrl = $"/lesson/{lesson.LessonID}",
                    VideoLength = lesson.VideoLength,
                    VisitCount = lesson.VisitCount,
                    CreateTime = lesson.CreateTime,
                });
            }

            foreach (var teachPlan in teachPlans)
            {
                searchs.Add(new SearchDto()
                {
                    Title = teachPlan.Name,
                    Description = $"{teachPlan.Domain} {teachPlan.Teacher ?? ""}",
                    Type = (int)SearchTypes.TeachPlan,
                    ImageUrl = "",
                    LinkUrl = $"/techplan/{teachPlan.TeachPlanID}",
                    VideoLength = 0,
                    VisitCount = 0,
                    CreateTime = teachPlan.CreateTime,
                });
            }

            searchs.OrderByDescending(x => x.CreateTime);
            return OK(searchs);
        }

        private IEnumerable<LessonDTO> GetLessonSearchResult(string keyword)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return dao.GetAll("LessonName LIKE @key OR Brief LIKE @key ORDER BY CreateTime DESC", new
                {
                    key = $"%{keyword}%"
                }).ToList();
            }
        }

        private IEnumerable<TeachPlanDTO> GetTeachPlanSearchResult(string keyword)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            {
                return dao.GetAll("Name LIKE @key ORDER BY CreateTime DESC", new
                {
                    key = $"%{keyword}%"
                }).ToList();
            }
        }
    }
}