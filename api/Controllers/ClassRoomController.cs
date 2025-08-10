namespace npm.api.API.Controllers
{
    using NLog;
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Linq;
    using System.Web.Http;

    [RoutePrefix("ClassRoom")]
    public partial class ClassRoomController : BaseController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string codes = "QAZWSXEDCRFVTGBYHNUJMIKOLP";
        private static int codeLength = 5;

        public ClassRoomController()
        {
        }

        [HttpGet]
        public ResponseDTO GetClassRoomCode()
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            {
                string code = string.Empty;

                for (int i = 0; i < 30; i++)
                {
                    code = GetRandomCode();
                    if (!dao.Any("Code = @Code", new { code }))
                    {
                        break;
                    }
                }

                return OK(code);
            }
        }

        [HttpGet]
        public ResponseDTO GetClassRoomByCode(string id)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            using (var ldao = GetDAO<LessonDAO>(dao))
            using (var mdao = GetDAO<MemberDAO>(dao))
            using (var mcdao = GetDAO<MemberClassRoomDAO>(dao))
            using (var lcdao = GetDAO<MemberHomeWorkDAO>(dao))
            {
                var room = dao.Get("Code = @Code", new { Code = id });
                if (room == null)
                {
                    return Error(Web.Status.NotFound);
                }

                var par = new
                {
                    LessonID = room.Lessons.ToArray(),
                    room.ClassRoomID,
                    Token.AccountID
                };

                var lessons = ldao
                    .GetAll("LessonID IN @LessonID", par)
                    .ToList();

                return OK(new
                {
                    ClassRoom = room,
                    Lessons = lessons,
                    Teacher = mdao.Get(room.TeacherMemberID),
                    HomeWorks = lcdao.GetAll("LessonID IN @LessonID AND MemberID = @AccountID", par),
                    Added = mcdao.Any("MemberID = @AccountID AND ClassRoomID = @ClassRoomID", par)
                });
            }
        }

        [HttpPost]
        public ResponseDTO AddMyClassRoom(int id)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            using (var mcdao = GetDAO<MemberClassRoomDAO>(dao))
            using (var ldao = GetDAO<LessonDAO>(dao))
            using (var mhdao = GetDAO<MemberHomeWorkDAO>(dao))
            using (var hdao = GetDAO<HomeWorkDAO>(dao))
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            {
                var room = dao.Get(id);
                if (mcdao.Any("MemberID = @AccountID AND ClassRoomID = @ClassRoomID", new { Token.AccountID, room.ClassRoomID }))
                {
                    return OK();
                }

                if (room.StartTime > DateTime.Now || room.EndTime < DateTime.Now)
                {
                    return Error(Web.Status.Expired);
                }

                var lessons = ldao.GetAll("LessonID IN @LessonIDs", new { LessonIDs = room.Lessons.ToArray() });
                var trans = dao.BeginTransaction();
                mcdao.Insert(new MemberClassRoomDTO()
                {
                    MemberID = Token.AccountID,
                    ClassRoomID = room.ClassRoomID,
                    Progress = 0
                }, trans);

                foreach (var lessonID in room.Lessons)
                {
                    mldao.Insert(new MemberLessonDTO()
                    {
                        MemberID = Token.AccountID,
                        LessonID = lessonID,
                        Progress = 0,
                        ClassRoomID = id
                    });
                }

                if (room.HasHomeWork)
                {
                    var homewroks = hdao.GetAll("LessonID IN @LessonIDs", new { LessonIDs = room.HomeWorks.ToArray() }, trans);
                    foreach (var l in homewroks)
                    {
                        mhdao.Insert(new MemberHomeWorkDTO()
                        {
                            MemberID = Token.AccountID,
                            TotalCount = l.Count,
                            LessonID = l.LessonID,
                            ClassRoomID = id
                        }, trans);
                    }
                }

                trans.Commit();
                return OK();
            }
        }

        [HttpPost]
        public ResponseDTO CreateClassRoom(ClassRoomDTO model)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            {
                model.TeacherMemberID = Token.AccountID;
                return OK(dao.Insert(model));
            }
        }

        [HttpPost]
        public ResponseDTO UpdateClassRoom(ClassRoomDTO model)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            {
                model.TeacherMemberID = Token.AccountID;
                return OK(dao.Update(model));
            }
        }

        [HttpGet]
        public ResponseDTO GetMyClassRoom()
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            using (var mdao = GetDAO<MemberDAO>(dao))
            using (var ldao = GetDAO<LessonDAO>(dao))
            using (var mcdao = GetDAO<MemberClassRoomDAO>(dao))
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            using (var mhdao = GetDAO<MemberHomeWorkDAO>(dao))
            {
                var member = mdao.Get(Token.AccountID);
                if (member.Type == MemberDTO.MemberType.Student)
                {
                    var mclist = mcdao.GetAll("MemberID = @AccountID", new { Token.AccountID });
                    var list = dao.GetAll("ClassRoomID IN @ClassRoomIDs", new
                    {
                        ClassRoomIDs = mclist
                            .Select(x => x.ClassRoomID)
                            .ToArray()
                    }).ToList();

                    var par = new
                    {
                        LessonIDs = list
                            .SelectMany(x => x.Lessons)
                            .ToArray(),
                        Token.AccountID
                    };

                    return OK(new
                    {
                        ClassRooms = list,
                        MyClassRooms = mclist,
                        MemberLesson = mldao.GetAll("MemberID = @AccountID", par),
                        HomeWorks = mhdao.GetAll("MemberID = @AccountID AND LessonID IN @LessonIDs", par),
                        Lessons = ldao.GetAll("LessonID IN @LessonIDs", par),
                        Members = mdao.GetAll("MemberID IN @MemberIDs", new { MemberIDs = list.Select(x => x.TeacherMemberID).ToArray() })
                    });
                }
                else
                {
                    var list = dao.GetAll("TeacherMemberID = @MemberID AND Status IN (0, 1) ORDER BY CreateTime DESC", member).ToList();
                    return OK(new
                    {
                        ClassRooms = list,
                        Lessons = ldao.GetAll("LessonID IN @LessonIDs", new
                        {
                            LessonIDs = list
                                .SelectMany(x => x.Lessons)
                                .ToArray()
                        })
                    });
                }
            }
        }

        [HttpGet]
        public ResponseDTO GetClassRoomTrace()
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            using (var mdao = GetDAO<MemberDAO>(dao))
            using (var ldao = GetDAO<LessonDAO>(dao))
            using (var mcdao = GetDAO<MemberClassRoomDAO>(dao))
            using (var mhdao = GetDAO<MemberHomeWorkDAO>(dao))
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            {
                var member = mdao.Get(Token.AccountID);
                if (member.Type != MemberDTO.MemberType.Teacher)
                {
                    return Error(Web.Status.Forbidden);
                }

                var list = dao.GetAll("TeacherMemberID = @MemberID AND Status IN (0, 1) ORDER BY CreateTime DESC", member).ToList();
                var lessonIDs = list
                    .SelectMany(x => x.Lessons)
                    .ToArray();

                return OK(new
                {
                    ClassRooms = list,
                    Lessons = ldao.GetAll("LessonID IN @LessonIDs", new
                    {
                        LessonIDs = lessonIDs
                    }),
                    FinishLesson = mldao.GetFinish(lessonIDs),
                    FinishHomeWork = mhdao.GetFinish(lessonIDs),
                    ClassRoomMemberCount = mcdao
                        .GetSummery(list.Select(x => x.ClassRoomID))
                        .ToDictionary(x => x.ClassRoomID, x => x.Count)
                });
            }
        }

        [HttpGet]
        public ResponseDTO GetClassRoomByLessonID(int id)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            using (var mdao = GetDAO<MemberDAO>(dao))
            using (var ldao = GetDAO<LessonDAO>(dao))
            using (var mldao = GetDAO<MemberLessonDAO>(dao))
            using (var mcdao = GetDAO<MemberClassRoomDAO>(dao))
            {
                var member = mdao.Get(Token.AccountID);
                if (member.Type == MemberDTO.MemberType.Student)
                {
                    var room = mldao.GetClassRoomByLessonID(id, Token.AccountID);
                    if (room == null)
                    {
                        return Error(Web.Status.NotFound);
                    }

                    return OK(new
                    {
                        ClassRoom = room,
                        Lessons = ldao.GetAll("LessonID IN @LessonIDs", new { LessonIDs = room.Lessons.ToArray() }),
                        MemberLesson = mldao.Get("MemberID = @AccountID AND LessonID = @id", new { Token.AccountID, id }),
                    });
                }

                return OK();
            }
        }

        [HttpGet]
        public ResponseDTO GetClassRoom(int id)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            {
                return OK(dao.Get(id));
            }
        }

        [HttpDelete]
        public ResponseDTO DeleteClassRoom(int id)
        {
            using (var dao = GetDAO<ClassRoomDAO>())
            {
                var room = dao.Get(id);
                room.Status = ClassRoomDTO.ClassRoomStatus.Deleted;
                return OK(dao.Update(room));
            }
        }

        private string GetRandomCode()
        {
            Random rnd = new Random();
            string code = string.Empty;

            for (var i = 0; i < codeLength; i++)
            {
                code += codes[rnd.Next(0, codes.Length - 1)];
            }

            return code;
        }
    }
}
