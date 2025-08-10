namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using Web.DAO;

    /// <summary>
    /// 課程章節
    /// </summary>
    public partial class LessonChapterDAO : OrmBaseDAO<LessonChapterDTO>
    {
        public int GetCount(IEnumerable<int> lessonIDs)
        {
            return this.conn.ExecuteScalar<int>("SELECT COUNT(1) FROM LessonChapter " +
                "WHERE LessonID IN @LessonIDs",
                new
                {
                    LessonIDs = lessonIDs.ToArray()
                });
        }
    }
}
