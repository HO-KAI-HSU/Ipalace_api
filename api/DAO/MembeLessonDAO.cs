namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using Web.DAO;

    /// <summary>
    /// 我的班級
    /// </summary>
    public partial class MemberLessonDAO : OrmBaseDAO<MemberLessonDTO>
    {
        public ClassRoomDTO GetClassRoomByLessonID(int lessonID, int memberID)
        {
            return this.conn.QueryFirstOrDefault<ClassRoomDTO>($"SELECT cr.* FROM MemberLesson ml, ClassRoom cr " +
                $"WHERE ml.ClassRoomID = cr.ClassRoomID AND LessonID = {lessonID} AND MemberID = {memberID}");
        }

        public Dictionary<string, int> GetFinish(IEnumerable<int> lessonIDs)
        {
            return this.conn.Query(@"SELECT ClassRoomID, LessonID, count(*) AS `Count` FROM MemberLesson 
                WHERE Progress = 100 AND LessonID IN @LessonIDs 
                GROUP BY LessonID, ClassRoomID",
                new
                {
                    LessonIDs = lessonIDs.ToArray()
                })
                .ToDictionary(x => $"{x.ClassRoomID}-{x.LessonID}", x => (int)x.Count);
        }
    }
}
