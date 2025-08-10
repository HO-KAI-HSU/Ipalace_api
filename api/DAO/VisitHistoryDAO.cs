namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System.Collections.Generic;
    using Web.DAO;

    /// <summary>
    ///
    /// </summary>
    public partial class VisitHistoryDAO : OrmBaseDAO<VisitHistoryDTO>
    {
        public IEnumerable<LessonDTO> GetHistoryLesson(int memberID)
        {
            return this.conn.Query<LessonDTO>("SELECT l.* FROM Lesson l, VisitHistory vh WHERE l.LessonID = vh.LessonID AND vh.MemberID = @MemberID ORDER BY vh.CreateTime DESC", new { memberID });
        }
    }
}
