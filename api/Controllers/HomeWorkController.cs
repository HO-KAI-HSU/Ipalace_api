namespace npm.api.API.Controllers
{
    using NLog;
    using npm.api.API.Models;
    using npm.api.DAO;
    using System;
    using System.Linq;
    using System.Web.Http;

    [RoutePrefix("HomeWork")]
    public partial class HomeWorkController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public HomeWorkController()
        {
        }

        [HttpGet]
        public ResponseDTO GetHomeWork(int id)
        {
            using (var dao = GetDAO<HomeWorkDAO>())
            using (var mhdao = GetDAO<MemberHomeWorkDAO>(dao))
            using (var ldao = GetDAO<LessonDAO>(dao))
            {
                var homework = dao.Get(id);
                if (homework == null)
                {
                    return OK();
                }

                var myHomework = mhdao.Get("MemberID = @AccountID AND LessonID = @id", new { Token.AccountID, id });
                if (myHomework.Answers == null)
                {
                    return OK(new
                    {
                        HomeWork = new
                        {
                            homework.Question
                        },
                        Lesson = ldao.Get(id)
                    });
                }

                return OK(new
                {
                    HomeWork = homework,
                    MyHomeWork = myHomework,
                    Lesson = ldao.Get(id)
                });
            }
        }

        [HttpPost]
        public ResponseDTO DoHomeWork(int id, [FromBody] HomeWorkModel model)
        {
            using (var dao = GetDAO<HomeWorkDAO>())
            using (var mhdao = GetDAO<MemberHomeWorkDAO>(dao))
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            using (var cdao = GetDAO<ClassRoomDAO>(dao))
            {
                var homework = dao.Get(id);
                if (homework == null)
                {
                    return Error(Web.Status.NotFound);
                }

                var myHomework = mhdao.Get("MemberID = @AccountID AND LessonID = @id", new { Token.AccountID, id });
                if (myHomework.Answers != null)
                {
                    return Error(Web.Status.Forbidden);
                }

                var room = mldao.GetClassRoomByLessonID(homework.LessonID, Token.AccountID);
                if (room.StartTime > DateTime.Now || room.EndTime < DateTime.Now)
                {
                    return Error(Web.Status.Expired);
                }

                int count = 0;
                var data = homework.Data.ToList();
                var answers = model.Answers.ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    if (answers.Count <= i)
                    {
                        break;
                    }

                    if (data[i].CorrectAnswer == answers[i])
                    {
                        count++;
                    }
                }

                myHomework.Answers = model.Answers;
                myHomework.CorrectCount = count;
                myHomework.TotalCount = data.Count;
                myHomework.FinishTime = DateTime.Now;
                mhdao.Update(myHomework);
                return OK(new
                {
                    HomeWork = homework,
                    MyHomeWork = myHomework
                });
            }
        }
    }
}
