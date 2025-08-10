namespace npm.api.DAO
{
    using Dapper;
    using npm.api.API.Models.Backend.Dto;
    using npm.api.DTO;
    using System.Collections.Generic;
    using Web.DAO;

    /// <summary>
    ///
    /// </summary>
    public partial class VMemberRegisterStatisticsDAO : OrmBaseDAO<VMemberRegisterStatisticsDTO>
    {
        public IEnumerable<MemberRegisterStatisticDto> GetRegisterCountByDateType(int dateType, string startDate, string endDate)
        {
            switch (dateType)
            {
                case 2:
                    return this.conn.Query<MemberRegisterStatisticDto>(@"SELECT DATE_FORMAT(Date,'%Y-%u') AS `Title`, Type, SUM(TotalCount) AS `Count` FROM VMemberRegisterStatistics Where Date >= @startDate AND Date <= @endDate GROUP BY Title, Type order by Title ASC", new
                    {
                        startDate,
                        endDate
                    });
                case 3:
                    return this.conn.Query<MemberRegisterStatisticDto>(@"SELECT DATE_FORMAT(Date,'%Y-%m') AS `Title`, Type, SUM(TotalCount) AS `Count` FROM VMemberRegisterStatistics Where Date >= @startDate AND Date <= @endDate GROUP BY Title, Type order by Title ASC", new
                    {
                        startDate,
                        endDate
                    });
                case 4:
                    return this.conn.Query<MemberRegisterStatisticDto>(@"SELECT DATE_FORMAT(Date,'%Y') AS `Title`, Type, SUM(TotalCount) AS `Count` FROM VMemberRegisterStatistics Where Date >= @startDate AND Date <= @endDate GROUP BY Title, Type order by Title ASC", new
                    {
                        startDate,
                        endDate
                    });
                default:
                    return this.conn.Query<MemberRegisterStatisticDto>(@"SELECT Date AS `Title`, Type, TotalCount AS `Count` FROM VMemberRegisterStatistics Where Date >= @startDate AND Date <= @endDate GROUP BY Title, Type order by Title ASC", new
                    {
                        startDate,
                        endDate
                    });
            }
        }

        public IEnumerable<MemberRegisterStatisticDto> GetRegisterTotalCountsByDate(string startDate, string endDate)
        {
            return this.conn.Query<MemberRegisterStatisticDto>(@"SELECT Date AS `Title`, TotalCount AS `Count` FROM VMemberRegisterStatistics Where Date >= @startDate AND Date <= @endDate Order By Date asc", new
            {
                startDate,
                endDate
            });
        }
    }
}
