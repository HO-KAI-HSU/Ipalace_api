namespace npm.api.API.Controllers
{
    using NLog;
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Http;
    using Web;
    using Web.Configs;

    [RoutePrefix("Lesson")]
    public partial class LessonController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public LessonController()
        {
        }

        [HttpGet]
        public ResponseDTO GetLessonCategories()
        {
            using (var dao = GetDAO<LessonCategoryDAO>())
            {
                return OK(dao.GetAll());
            }
        }

        [HttpGet]
        public ResponseDTO GetLessons()
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetAll("Active = @active and IsDeleted = @isDeleted", new
                {
                    active = true,
                    isDeleted = false
                }));
            }
        }

        [HttpGet]
        public ResponseDTO Search(string id)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetAll("Active = @active AND LessonName LIKE @key OR Brief LIKE @key ORDER BY CreateTime DESC", new
                {
                    active = true,
                    key = $"%{id}%"
                }));
            }
        }

        [HttpGet]
        public ResponseDTO GetLesson(string id)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                var lessons = dao.GetAll("LessonID IN @ids", new
                {
                    ids = id.Split(',')
                });

                if (lessons.Count() == 0)
                {
                    return Error(Status.NotFound);
                }

                return OK(lessons);
            }
        }

        [HttpGet]
        public ResponseDTO GetLessonByCategory(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                var lessons = dao.GetAll("Active = 1 and IsDeleted = 0 and LessonCategoryID = " + id + " ORDER BY CreateTime DESC");

                if (lessons.Count() == 0)
                {
                    return Error(Status.NotFound);
                }

                return OK(lessons);
            }
        }

        [HttpGet]
        public ResponseDTO GetStudentLessons()
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetAll("Active = 1 and IsDeleted = 0 and LessonCategoryID <> 1 ORDER BY CreateTime DESC"));
            }
        }

        [HttpGet]
        public ResponseDTO GetChapters(int id)
        {
            using (var dao = GetDAO<LessonChapterDAO>())
            using (var ldao = GetDAO<LessonDAO>(dao))
            {
                var chapter = dao.Get(id);

                if (chapter == null)
                {
                    return Error(Status.NotFound);
                }

                var lesson = ldao.Get(chapter.LessonID);
                return OK(new
                {
                    Lesson = lesson,
                    Chapters = dao.GetAll("LessonID = " + chapter.LessonID)
                });
            }
        }

        [HttpGet]
        public ResponseDTO GetFullLesson(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var lcdao = GetDAO<LessonChapterDAO>(dao))
            {
                var lesson = dao.Get(id);
                if (lesson == null)
                {
                    return Error(Status.NotFound);
                }

                return OK(new
                {
                    Lesson = lesson,
                    Chapters = lcdao.GetAll("LessonID = @LessonID", new { LessonID = id })
                });
            }
        }

        [HttpGet]
        public ResponseDTO GetLearnedChapter(int id)
        {
            using (var dao = GetDAO<MemberLessonChapterDAO>())
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            {
                var room = mldao.GetClassRoomByLessonID(id, Token.AccountID);
                return OK(dao.GetAll($"ClassRoomID = {room.ClassRoomID} AND MemberID = {Token.AccountID} AND LessonID = {id}"));
            }
        }

        [HttpGet]
        public ResponseDTO GetRecommand()
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetRandom(9));
            }
        }

        [HttpGet]
        public ResponseDTO GetNew()
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetAll("Active = 1 and IsDeleted = 0 ORDER BY CreateTime DESC"));
            }
        }

        [HttpGet]
        public ResponseDTO GetHot()
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetAll("Active = 1 and IsDeleted = 0 ORDER BY CreateTime DESC"));
            }
        }

        [HttpGet]
        public ResponseDTO GetHistory()
        {
            using (var dao = GetDAO<VLessonChapterHistoryDAO>())
            {
                var vLessonChapterHistorys = dao.GetAll("MemberID = @AccountID", new { Token.AccountID });
                var lessonChapterHistorys = vLessonChapterHistorys
                    .GroupBy(x => x.LessonID)
                    .Select(x => new
                    {
                        LessonID = x.Key,
                        x.FirstOrDefault().LessonName,
                        x.FirstOrDefault().Brief,
                        x.FirstOrDefault().ImageUrl,
                        x.FirstOrDefault().VideoLength,
                        ChapterSubjects = x
                            .GroupBy(y => y.LessonSubjectID)
                            .Select(y => new
                            {
                                y.Key,
                                y.LastOrDefault().LessonSubjectName,
                                ChapterHistorys = y
                                .Select(z => new
                                {
                                    z.LessonChapterID,
                                    z.ChapterName,
                                    z.ChapterVideoLength,
                                    z.Time,
                                    z.Done
                                }).ToArray()
                            }).ToArray()
                    }).ToArray();

                return OK(lessonChapterHistorys);
            }
        }

        [HttpPost]
        public ResponseDTO Visit(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var vhdao = GetDAO<VisitHistoryDAO>(dao))
            using (var mlldao = GetDAO<MemberLessonLogDAO>(dao))
            {
                var memberId = Token == null ? 0 : Token.AccountID;
                var visit = vhdao.Get("MemberID = @AccountID AND LessonID = @id", new { AccountID = memberId, id });
                if (visit == null)
                {
                    vhdao.Insert(new DTO.VisitHistoryDTO()
                    {
                        MemberID = memberId,
                        LessonID = id,
                    });
                }
                else
                {
                    visit.CreateTime = DateTime.Now;
                    vhdao.Update(visit);
                }

                dao.UpdateVisitCount(id, 1);

                mlldao.Insert(new DTO.MemberLessonLogDTO()
                {
                    MemberID = memberId,
                    LessonID = id,
                    Type = 1,
                    ClientIp = null,
                    CreateTime = DateTime.Now
                });
            }

            return OK();
        }

        [HttpGet]
        public ResponseDTO GetMyVisit()
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var vhdao = GetDAO<VisitHistoryDAO>(dao))
            {
                var list = vhdao.GetAll("MemberID = @AccountID ORDER BY CreateTime DESC", new { Token.AccountID }).ToList();
                return OK(dao.GetAll("LessonID IN @LessonID", new { LessonID = list.Select(x => x.LessonID).ToArray() }));
            }
        }

        [HttpPost]
        public ResponseDTO Learn(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            using (var lcdao = GetDAO<LessonChapterDAO>(dao))
            using (var mcdao = GetDAO<MemberClassRoomDAO>(dao))
            using (var mlcdao = GetDAO<MemberLessonChapterDAO>(dao))
            using (var cdao = GetDAO<ClassRoomDAO>(dao))
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            {
                var learned = mlcdao.Get("MemberID = @AccountID AND LessonChapterID = @id", new { Token.AccountID, id });
                if (learned != null)
                {
                    return OK();
                }

                var chapter = lcdao.Get(id);
                var lesson = dao.Get(chapter.LessonID);
                var room = mldao.GetClassRoomByLessonID(lesson.LessonID, Token.AccountID);
                if (room.StartTime > DateTime.Now ||
                    room.EndTime < DateTime.Now)
                {
                    return Error(Web.Status.Expired);
                }

                var mroom = mcdao.Get("MemberID = @AccountID AND ClassRoomID = @ClassRoomID", new { Token.AccountID, room.ClassRoomID });
                mlcdao.Insert(new DTO.MemberLessonChapterDTO
                {
                    LessonChapterID = id,
                    MemberID = Token.AccountID,
                    LessonID = lesson.LessonID,
                    ClassRoomID = room.ClassRoomID
                });
                double finished = mlcdao.GetCount(Token.AccountID, room.Lessons);
                double total = lcdao.GetCount(room.Lessons);
                if (finished == total)
                {
                    mroom.Progress = 100;
                    mroom.FinishTime = DateTime.Now;
                }
                else
                {
                    mroom.Progress = (int)Math.Floor(finished / total * 100.0);
                }

                mcdao.Update(mroom);

                var mlesson = mldao.Get("MemberID = @AccountID AND LessonID = @LessonID", new { Token.AccountID, lesson.LessonID });
                finished = mlcdao.GetCount(Token.AccountID, new int[] { lesson.LessonID });
                total = lcdao.GetCount(new int[] { lesson.LessonID });
                if (finished == total)
                {
                    mlesson.Progress = 100;
                    mlesson.FinishTime = DateTime.Now;
                }
                else
                {
                    mlesson.Progress = (int)Math.Floor(finished / total * 100.0);
                }

                mldao.Update(mlesson);
                return OK(mlesson.Progress);
            }
        }

        [HttpGet]
        public ResponseDTO GetLessonTraceDetail(int id, [FromUri] int cid)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                return OK(dao.GetTraceSummery(id, cid));
            }
        }

        [HttpGet]
        public ResponseDTO GetChapterProgress(int id)
        {
            using (var dao = GetDAO<MemberLessonChapterProgressDAO>())
            {
                var memberId = Token == null ? 0 : Token.AccountID;
                return OK(dao.Get(memberId, id));
            }
        }

        [HttpPost]
        public ResponseDTO UpdateChapterProgress([FromBody] MemberLessonChapterProgressDTO model)
        {
            using (var dao = GetDAO<MemberLessonChapterProgressDAO>())
            {
                var memberId = Token == null ? 0 : Token.AccountID;
                var dto = dao.Get(memberId, model.LessonChapterID);
                if (dto == null)
                {
                    dto = new MemberLessonChapterProgressDTO()
                    {
                        MemberID = memberId,
                        LessonChapterID = model.LessonChapterID,
                    };
                }

                dto.Time = model.Time;
                dto.Done = model.Done;

                return OK(dao.Replace(dto));
            }
        }

        [HttpPost]
        public ResponseDTO UpdateLessonLog([FromBody] MemberLessonChapterProgressDTO model)
        {
            var queryString = Request
                .GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => x.Value);

            if (queryString.TryGetValue("ip", out string ip))
            {
            }

            using (var dao = GetDAO<MemberLessonLogDAO>())
            using (var lcdao = GetDAO<LessonChapterDAO>(dao))
            using (var mlcfldao = GetDAO<MemberLessonChapterFinishedLogDAO>(dao))
            {
                var memberId = Token == null ? 0 : Token.AccountID;
                var lcdto = lcdao.Get("LessonChapterID = @id", new { id = model.LessonChapterID });
                var mlldto = dao.GetAll("MemberID = @id AND LessonChapterID = @chapterId AND ClientIp = @clientIp AND Type = 2", new { id = memberId, chapterId = model.LessonChapterID, clientIp = ip })
                    .OrderByDescending(x => x.CreateTime)
                    .FirstOrDefault();
                try
                {
                    if (model.Done)
                    {
                        mlcfldao.Insert(new DTO.MemberLessonChapterFinishedLogDTO()
                        {
                            MemberID = memberId,
                            LessonID = lcdto.LessonID,
                            LessonChapterID = model.LessonChapterID,
                            CreateTime = DateTime.Now
                        });
                    }
                }
                catch (Exception e)
                {
                }

                if (mlldto == null || (mlldto.CreateTime.AddSeconds(10) < DateTime.Now))
                {
                    dao.Insert(new DTO.MemberLessonLogDTO()
                    {
                        MemberID = memberId,
                        LessonID = lcdto.LessonID,
                        LessonChapterID = model.LessonChapterID,
                        ClientIp = ip,
                        Type = 2,
                        Time = model.Time,
                        CreateTime = DateTime.Now
                    });
                }
                else
                {
                    mlldto.Time = model.Time;
                    mlldto.CreateTime = DateTime.Now;
                    dao.Replace(mlldto);
                }

                return OK();
            }
        }

        [HttpGet]
        public HttpResponseMessage Share(int id)
        {
            using (var dao = GetDAO<LessonDAO>())
            {
                var lesson = dao.Get(id);
                var brief = "";
                if (!string.IsNullOrEmpty(lesson.Brief))
                {
                    brief = Regex.Replace(lesson.Brief.Replace("\n", ""), "<.*?>", "");
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(
                       $@"<html><header>
                        <meta charset=""utf-8""/>
                        <meta property=""og:url"" content=""{Config.Instance.FrontendUrl}/lesson/{id}"" />
                        <meta property=""og:type"" content=""article"" />
                        <meta property=""og:title"" content=""{lesson.LessonName}"" />
                        <meta property=""og:description"" content=""{brief}"" />
                        <meta property=""og:image"" content=""{Config.Instance.CDNUrl}/{lesson.ImageUrl}"" />
                        </header>
                        <body>
                        <script> window.location.href = ""{Config.Instance.FrontendUrl}/lesson/{id}""; </script>
                        </body></html>",
                        Encoding.UTF8,
                        "text/html")
                };
            }
        }
    }
}
