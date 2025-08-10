namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using Web.DAO;

    /// <summary>
    ///
    /// </summary>
    public partial class MemberLessonChapterDAO : OrmBaseDAO<MemberLessonChapterDTO>
    {
        public int GetCount(int memberID, IEnumerable<int> lessonIDs)
        {
            return this.conn.ExecuteScalar<int>("SELECT COUNT(1) FROM MemberLessonChapter " +
                "WHERE MemberID = @MemberID AND LessonID IN @LessonIDs",
                new
                {
                    memberID,
                    LessonIDs = lessonIDs.ToArray()
                });
        }
    }
}
