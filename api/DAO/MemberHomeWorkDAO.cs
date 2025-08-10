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
    public partial class MemberHomeWorkDAO : OrmBaseDAO<MemberHomeWorkDTO>
    {
        public Dictionary<string, int> GetFinish(IEnumerable<int> lessonIDs)
        {
            return this.conn.Query(@"SELECT ClassRoomID, LessonID, count(*) AS `Count` FROM MemberHomeWork 
                WHERE FinishTime IS NOT NULL AND LessonID IN @LessonIDs 
                GROUP BY LessonID, ClassRoomID",
                new
                {
                    LessonIDs = lessonIDs.ToArray()
                })
                .ToDictionary(x => $"{x.ClassRoomID}-{x.LessonID}", x => (int)x.Count);
        }
    }
}
