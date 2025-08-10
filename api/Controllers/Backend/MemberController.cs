namespace npm.api.API.Controllers
{
    using npm.api.API.Helper;
    using npm.api.API.Models.Backend;
    using npm.api.API.Models.Backend.Dto;
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;

    public partial class BackendController : BaseController
    {
        [HttpGet]
        public ResponseDTO GetMembers()
        {
            using (var dao = GetDAO<MemberDAO>())
            {
                return OK(dao.GetAll("MemberID > 0"));
            }
        }

        [HttpGet]
        public ResponseDTO GetMember(int id)
        {
            using (var dao = GetDAO<MemberDAO>())
            using (var vLessonChapterHistorydao = GetDAO<VLessonChapterHistoryDAO>(dao))
            {
                var vLessonChapterHistorys = vLessonChapterHistorydao.GetAll("MemberID = @id", new { id });
                var lessonChapterHistorys = vLessonChapterHistorys
                    .GroupBy(x => x.LessonID)
                    .Select(x => new
                    {
                        LessonID = x.Key,
                        x.FirstOrDefault().LessonName,
                        x.FirstOrDefault().Brief,
                        x.FirstOrDefault().ImageUrl,
                        x.FirstOrDefault().VideoLength,
                        LessonVisitCreateTime = x.FirstOrDefault().CreateTime,
                        LessonTotalVisitTime = x.Sum(y => y.Time),
                    }).ToArray();

                return OK(new
                {
                    Member = dao.Get(id),
                    Lessons = lessonChapterHistorys
                });
            }
        }

        [HttpPost]
        public ResponseDTO UpdateMember([FromBody] MemberDTO model)
        {
            using (var dao = GetDAO<MemberDAO>())
            {
                var member = dao.Get(model.MemberID);
                member.Name = model.Name;
                member.Mobile = model.Mobile;
                member.School = model.School;
                member.Class = model.Class;
                member.Status = model.Status;
                member.Type = model.Type;
                member.Email = model.Email;
                dao.Update(model);
                return OK();
            }
        }

        [HttpPost]
        public ResponseDTO UpdateMemberPassword([FromBody] MemberDTO model)
        {
            using (var dao = GetDAO<MemberDAO>())
            {
                if (!string.IsNullOrEmpty(model.Password))
                {
                    dao.UpdatePassword(model.MemberID, MemberController.EncodePassword(model.Password));
                }

                return OK();
            }
        }

        [HttpGet]
        public ResponseDTO GetMemberLessonStatisticList()
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

            using (var dao = GetDAO<MemberDAO>())
            using (var vmsdao = GetDAO<VMemberLessonStatisticsDAO>(dao))
            {
                var members = dao.GetAll();
                var statistics = (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) ?
                    vmsdao.GetAll()
                    .GroupBy(x => new { x.MemberID })
                    .Select(x => new MemberLessonStatisticModel
                    {
                        MemberID = x.Key.MemberID,
                        TotalCount = x.Where(y => y.Type == 1).Sum(y => y.TotalCount),
                        TotalTime = x.Where(y => y.Type == 2).Sum(y => y.TotalTime)
                    }) :
                    vmsdao.GetAll("Date >= @Start AND Date <= @End", new { Start = startDate, End = endDate })
                    .GroupBy(x => new { x.MemberID })
                    .Select(x => new MemberLessonStatisticModel
                    {
                        MemberID = x.Key.MemberID,
                        TotalCount = x
                            .Where(y => y.Type == 1)
                            .Sum(y => y.TotalCount),
                        TotalTime = x
                            .Where(y => y.Type == 2)
                            .Sum(y => y.TotalTime)
                    });

                var memberLessonSummaryDto = new MemberLessonSummaryDto()
                {
                    AnonymousTotalCount = statistics
                        .Where(x => x.MemberID == 0)
                        .Select(x => x.TotalCount)
                        .FirstOrDefault(),
                    AnonymousTotalTime = statistics
                        .Where(x => x.MemberID == 0)
                        .Select(x => x.TotalTime)
                        .FirstOrDefault(),
                    MemberTotalCount = statistics
                        .Where(x => x.MemberID > 0)
                        .Sum(x => x.TotalCount),
                    MemberTotalTime = statistics
                        .Where(x => x.MemberID > 0)
                        .Sum(x => x.TotalTime),
                    MemberLessonStatisticList = new List<MemberLessonStatisticDto>()
                };

                var lessonStatistics = members
                    .Join(statistics,
                        x => x.MemberID,
                        y => y.MemberID,
                        (x, y) => new { Member = x, MemberLessonStatistic = y })
                    .Where(x =>
                        x.Member.MemberID > 0 &&
                        x.Member.Name != null)
                    .Select(x => new MemberLessonStatisticDto()
                    {
                        MemberID = x.Member.MemberID,
                        MemberName = x.Member.Name,
                        School = x.Member.School,
                        TotalCount = x.MemberLessonStatistic.TotalCount,
                        TotalTime = (float)Math.Floor((float)x.MemberLessonStatistic.TotalTime),
                    })
                    .OrderByDescending(x => CommonHelper.GetPropertyValue(x, orderBy));

                memberLessonSummaryDto.MemberLessonStatisticList = lessonStatistics.ToList();

                return OK(memberLessonSummaryDto);
            }
        }

        [HttpGet]
        public ResponseDTO GetMemberLessonRankingList()
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

            using (var dao = GetDAO<MemberDAO>())
            using (var vmsdao = GetDAO<VMemberLessonStatisticsDAO>(dao))
            {
                var members = dao.GetAll();
                var statistics = (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) ?
                    vmsdao.GetAll()
                    .GroupBy(x => new { x.MemberID })
                    .Select(x => new MemberLessonStatisticModel
                    {
                        MemberID = x.Key.MemberID,
                        TotalCount = x.Where(y => y.Type == 1).Sum(y => y.TotalCount),
                        TotalTime = x.Where(y => y.Type == 2).Sum(y => y.TotalTime)
                    }) :
                    vmsdao.GetAll("Date >= @Start AND Date <= @End", new { Start = startDate, End = endDate })
                    .GroupBy(x => new { x.MemberID })
                    .Select(x => new MemberLessonStatisticModel
                    {
                        MemberID = x.Key.MemberID,
                        TotalCount = x.Where(y => y.Type == 1).Sum(y => y.TotalCount),
                        TotalTime = x.Where(y => y.Type == 2).Sum(y => y.TotalTime)
                    });

                var lessonStatistics = members
                    .Join(statistics,
                        x => x.MemberID,
                        y => y.MemberID,
                        (x, y) => new { Member = x, MemberLessonStatistic = y })
                    .Where(x =>
                        x.Member.MemberID > 0 &&
                        x.Member.Name != null)
                    .Select(x => new MemberLessonStatisticDto()
                    {
                        MemberID = x.Member.MemberID,
                        MemberName = x.Member.Name,
                        School = x.Member.School,
                        TotalCount = x.MemberLessonStatistic.TotalCount,
                        TotalTime = (float)Math.Floor((float)x.MemberLessonStatistic.TotalTime),
                    })
                    .OrderByDescending(x => CommonHelper.GetPropertyValue(x, orderBy));

                return OK(lessonStatistics.Take(10));
            }
        }

        [HttpGet]
        public ResponseDTO GetMemberTeachPlanStatisticList()
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
            }

            if (queryString.TryGetValue("limit", out string limit))
            {
            }

            using (var dao = GetDAO<MemberDAO>())
            using (var vmsdao = GetDAO<VMemberTeachPlanStatisticsDAO>(dao))
            {
                var members = dao.GetAll();
                var statistics = (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate)) ?
                    vmsdao.GetAll()
                    .GroupBy(x => new { x.MemberID })
                    .Select(x => new MemberTeachPlanStatisticModel
                    {
                        MemberID = x.Key.MemberID,
                        TotalCount = x.Where(y => y.Type == 2).Sum(y => y.TotalCount)
                    }) :
                    vmsdao.GetAll("Date >= @Start AND Date <= @End", new { Start = startDate, End = endDate })
                    .GroupBy(x => new { x.MemberID })
                    .Select(x => new MemberTeachPlanStatisticModel
                    {
                        MemberID = x.Key.MemberID,
                        TotalCount = x.Where(y => y.Type == 2).Sum(y => y.TotalCount)
                    });

                var lessonStatistics = members
                    .Where(x => x.MemberID > 0)
                    .Join(statistics,
                        x => x.MemberID,
                        y => y.MemberID,
                        (x, y) => new { Member = x, MemberTeachPlanStatistic = y })
                    .Select(x => new MemberTeachPlanStatisticDto()
                    {
                        MemberID = x.Member.MemberID,
                        MemberName = x.Member.Name,
                        School = x.Member.School,
                        TotalCount = x.MemberTeachPlanStatistic.TotalCount
                    })
                    .OrderByDescending(x => x.TotalCount);

                return OK(lessonStatistics.Take(10));
            }
        }

        [HttpGet]
        public ResponseDTO GetMemberRegisterStatisticList()
        {
            var queryString = Request
                .GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => x.Value);

            if (queryString.TryGetValue("startDate", out string startDate))
            {
            }

            if (queryString.TryGetValue("endDate", out string endDate))
            {
            }

            if (queryString.TryGetValue("dateType", out string dateType))
            {
            }

            var type = dateType != null ? Convert.ToInt16(dateType) : 1;

            using (var vmsdao = GetDAO<VMemberRegisterStatisticsDAO>())
            {
                DateTime s;
                DateTime e;
                var start = !string.IsNullOrEmpty(startDate) ? startDate : DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
                var end = !string.IsNullOrEmpty(endDate) ? endDate : DateTime.Now.ToString("yyyy-MM-dd");
                var registerCounts = vmsdao.GetRegisterCountByDateType(type, start, end);

                var newRegisterCountKeyValues = registerCounts
                    .GroupBy(x => x.Title)
                    .Select(x => new MemberRegisterStatisticDto
                    {
                        Title = x.Key,
                        GeneralTotalRegisterCount = x
                            .Where(y => y.Type == 0)
                            .Sum(y => y.Count),
                        StudentTotalRegisterCount = x
                            .Where(y => y.Type == 2)
                            .Sum(y => y.Count),
                        TeacherTotalRegisterCount = x
                            .Where(y => y.Type == 1)
                            .Sum(y => y.Count),
                        Count = x.Sum(y => y.Count),
                    });

                var sumCount = 0;

                var dateRegisterCountsResult = newRegisterCountKeyValues.ToList();
                for (int i = 0; i < dateRegisterCountsResult.Count; i++)
                {
                    sumCount = sumCount + dateRegisterCountsResult[i].Count;
                    dateRegisterCountsResult[i].TotalRegisterCount = sumCount;
                }

                var dateRegisterCountsResultDV = dateRegisterCountsResult
                    .ToDictionary(x => x.Title, x => x);

                DateTime.TryParseExact(start, "yyyy-MM-dd", null, DateTimeStyles.None, out s);
                DateTime.TryParseExact(end, "yyyy-MM-dd", null, DateTimeStyles.None, out e);
                var memberRegisterStatistics = new List<MemberRegisterStatisticDto>();
                var initDataList = GetInitDataListByDataType(s, e, type);
                var currentSumCount = 0;
                var registerCountResult = initDataList
                    .Select(x =>
                    {
                        dateRegisterCountsResultDV.TryGetValue(x.Title, out MemberRegisterStatisticDto memberRegisterStatisticDto);

                        var titleRegisterCount = memberRegisterStatisticDto != null ?
                            memberRegisterStatisticDto.Count : 0;
                        var generalCount = memberRegisterStatisticDto != null ?
                            memberRegisterStatisticDto.GeneralTotalRegisterCount : 0;
                        var studentCount = memberRegisterStatisticDto != null ?
                            memberRegisterStatisticDto.StudentTotalRegisterCount : 0;
                        var teacherCount = memberRegisterStatisticDto != null ?
                            memberRegisterStatisticDto.TeacherTotalRegisterCount : 0;
                        var totalRegisterCount = memberRegisterStatisticDto != null ?
                            memberRegisterStatisticDto.TotalRegisterCount : 0;

                        currentSumCount = (totalRegisterCount > currentSumCount) ? totalRegisterCount : currentSumCount;

                        x.Count = titleRegisterCount;
                        x.GeneralTotalRegisterCount = generalCount;
                        x.StudentTotalRegisterCount = studentCount;
                        x.TeacherTotalRegisterCount = teacherCount;
                        x.TotalRegisterCount = currentSumCount;
                        return x;
                    });
                memberRegisterStatistics = registerCountResult.ToList();

                var preCount = 0;
                var growthRate = 0;
                for (int i = 0; i < memberRegisterStatistics.Count; i++)
                {
                    var nowCount = memberRegisterStatistics[i].Count;
                    if (i == 0)
                    {
                        growthRate = 0;
                    }
                    else
                    {
                        if (preCount != 0)
                        {
                            growthRate = (int)(((float)(nowCount - preCount) / (float)preCount) * 100);
                        }
                        else if (preCount == 0 && nowCount != 0)
                        {
                            growthRate = 100;
                        }
                        else
                        {
                            growthRate = 0;
                        }
                    }

                    memberRegisterStatistics[i].GrowthRate = growthRate;
                    preCount = memberRegisterStatistics[i].Count;
                }

                var res = new
                {
                    StartDate = start,
                    EndDate = end,
                    MemberRegisterStatistics = memberRegisterStatistics,
                };

                return this.OK(res);
            }
        }

        [HttpGet]
        public ResponseDTO GetMemberRegisterTotalCountsByDate()
        {
            var queryString = this.Request
                .GetQueryNameValuePairs()
                .ToDictionary(x => x.Key, x => x.Value);

            if (queryString.TryGetValue("startDate", out string startDate))
            {
            }

            if (queryString.TryGetValue("endDate", out string endDate))
            {
            }

            if (queryString.TryGetValue("dateType", out string dateType))
            {
            }

            var type = dateType != null ? Convert.ToInt16(dateType) : 1;

            using (var vmsdao = this.GetDAO<VMemberRegisterStatisticsDAO>())
            {
                var start = "1970-01-01";
                var end = DateTime.Now.ToString("yyyy-MM-dd");
                var registerCounts = vmsdao.GetRegisterTotalCountsByDate(start, end);
                var counts = registerCounts.Sum(x => x.Count);
                var res = new
                {
                    RegisterCounts = counts,
                };
                return this.OK(res);
            }
        }

        private IEnumerable<MemberRegisterStatisticDto> GetInitDataListByDataType(DateTime s, DateTime e, int dateType = 1)
        {
            switch (dateType)
            {
                case 2:
                    {
                        return this.GetWeekInitDataList(s, e);
                    }

                case 3:
                    {
                        return GetMonthInitDataList(s, e);
                    }

                case 4:
                    {
                        return GetYearInitDataList(s, e);
                    }

                default:
                    {
                        return GetDateInitDataList(s, e);
                    }
            }
        }

        private IEnumerable<MemberRegisterStatisticDto> GetDateInitDataList(DateTime startDate, DateTime endDate)
        {
            var list = new List<MemberRegisterStatisticDto>();
            while (startDate <= endDate)
            {
                var s = startDate.ToString("yyyy-MM-dd");
                var info = new MemberRegisterStatisticDto()
                {
                    Title = s,
                    Count = 0
                };
                list.Add(info);
                startDate = startDate.AddDays(1);
            }

            return list;
        }

        private IEnumerable<MemberRegisterStatisticDto> GetWeekInitDataList(DateTime startDate, DateTime endDate)
        {
            var list = new List<MemberRegisterStatisticDto>();
            var sYear = startDate.Year;
            var eYear = endDate.Year;

            var sWeek = this.GetWeekOfYear(startDate);
            var eWeek = this.GetWeekOfYear(endDate);

            if (sYear == eYear)
            {
                while (sWeek <= eWeek)
                {
                    var title = sYear + "-" + sWeek.ToString().PadLeft(2, '0');
                    var info = new MemberRegisterStatisticDto()
                    {
                        Title = title,
                        Count = 0,
                    };
                    list.Add(info);
                    sWeek += 1;
                }
            }
            else
            {
                DateTime.TryParseExact(sYear + "-12-31", "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime s);
                while ((sYear == eYear && sWeek <= eWeek) || (sYear != eYear && sWeek <= this.GetWeekOfYear(s)))
                {
                    var title = sYear + "-" + sWeek.ToString().PadLeft(2, '0');
                    var info = new MemberRegisterStatisticDto()
                    {
                        Title = title,
                        Count = 0
                    };
                    list.Add(info);

                    if ((sYear != eYear) && sWeek == this.GetWeekOfYear(s))
                    {
                        sYear += 1;
                        sWeek = 1;
                    }
                    else
                    {
                        sWeek += 1;
                    }
                }
            }

            return list;
        }

        private IEnumerable<MemberRegisterStatisticDto> GetMonthInitDataList(DateTime startDate, DateTime endDate)
        {
            var list = new List<MemberRegisterStatisticDto>();
            var sYear = startDate.Year;
            var eYear = endDate.Year;
            var sMonth = startDate.Month;
            var eMonth = endDate.Month;

            if (sYear == eYear)
            {
                while (sMonth <= eMonth)
                {
                    var title = sYear + "-" + sMonth.ToString().PadLeft(2, '0');
                    var info = new MemberRegisterStatisticDto()
                    {
                        Title = title,
                        Count = 0
                    };
                    list.Add(info);
                    sMonth += 1;
                }
            }
            else
            {
                DateTime.TryParseExact(sYear + "-12-31", "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime s);
                while ((sYear == eYear && sMonth <= eMonth) || (sYear != eYear && sMonth <= s.Month))
                {
                    var title = sYear + "-" + sMonth.ToString().PadLeft(2, '0');
                    var info = new MemberRegisterStatisticDto()
                    {
                        Title = title,
                        Count = 0
                    };
                    list.Add(info);

                    if ((sYear != eYear) && sMonth == s.Month)
                    {
                        sYear += 1;
                        sMonth = 1;
                    }
                    else
                    {
                        sMonth += 1;
                    }
                }
            }

            return list;
        }

        private IEnumerable<MemberRegisterStatisticDto> GetYearInitDataList(DateTime startDate, DateTime endDate)
        {
            var list = new List<MemberRegisterStatisticDto>();
            var sYear = startDate.Year;
            var eYear = endDate.Year;

            while (sYear <= eYear)
            {
                var title = sYear.ToString();
                var info = new MemberRegisterStatisticDto()
                {
                    Title = title,
                    Count = 0
                };
                list.Add(info);
                sYear += 1;
            }

            return list;
        }

        private int GetWeekOfYear(DateTime date)
        {
            GregorianCalendar gregorianCalendar = new GregorianCalendar();
            var weekOfYear = gregorianCalendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }
    }
}