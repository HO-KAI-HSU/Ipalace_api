namespace npm.api.API.Controllers
{
    using NLog;
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Linq;
    using System.Web.Http;

    [RoutePrefix("TeachPlan")]
    public partial class TeachPlanController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public TeachPlanController()
        {
        }

        [HttpGet]
        public ResponseDTO GetTeachPlanCategories()
        {
            using (var dao = GetDAO<TeachPlanCategoryDAO>())
            {
                var result = dao
                    .GetAll()
                    .OrderBy(x => x.Sort)
                    .ThenBy(x => x.TeachPlanCategoryID);

                return OK(result);
            }
        }

        [HttpGet]
        public ResponseDTO GetTeachPlans()
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            {
                return OK(dao.GetAll("Active = @active and IsDeleted = @isDeleted", new { active = true, isDeleted = false }));
            }
        }

        [HttpGet]
        public ResponseDTO GetTeachPlan(int id)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            using (var ldao = GetDAO<LessonDAO>(dao))
            {
                var plan = dao.Get(id);

                if (plan == null)
                {
                    return Error(Web.Status.NotFound);
                }

                LessonDTO lesson = null;
                if (plan.LessonID.HasValue)
                {
                    lesson = ldao.Get(plan.LessonID.Value);
                }

                return OK(new
                {
                    TeachPlan = plan,
                    Lesson = lesson
                });
            }
        }

        [HttpGet]
        public ResponseDTO GetTeachPlanFiles(int id)
        {
            using (var dao = GetDAO<TeachPlanFileDAO>())
            {
                var result = dao.GetAll("Active = @active and TeachPlanID = @id", new { active = true, id });
                return OK(result);
            }
        }

        [HttpPut]
        public ResponseDTO UpdateVisitCount(int id)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            using (var mtpldao = GetDAO<MemberTeachPlanLogDAO>(dao))
            {
                var plan = dao.Get(id);
                if (plan == null)
                {
                    return Error(Web.Status.NotFound);
                }

                mtpldao.Insert(new DTO.MemberTeachPlanLogDTO()
                {
                    MemberID = Token == null ? 0 : Token.AccountID,
                    TeachPlanID = id,
                    Type = 1,
                    CreateTime = DateTime.Now
                });

                dao.UpdateVisitCount(id, 1);
            }

            return OK();
        }

        [HttpPut]
        public ResponseDTO UpdateDownloadCount(int id)
        {
            using (var dao = GetDAO<TeachPlanDAO>())
            using (var mtpldao = GetDAO<MemberTeachPlanLogDAO>(dao))
            {
                var plan = dao.Get(id);
                if (plan == null)
                {
                    return Error(Web.Status.NotFound);
                }

                mtpldao.Insert(new DTO.MemberTeachPlanLogDTO()
                {
                    MemberID = Token == null ? 0 : Token.AccountID,
                    TeachPlanID = id,
                    Type = 2,
                    CreateTime = DateTime.Now
                });

                dao.UpdateDownloadCount(id, 1);
            }

            return OK();
        }
    }
}