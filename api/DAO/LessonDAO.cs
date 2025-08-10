namespace npm.api.DAO
{
    using Dapper;
    using npm.api.API.Models;
    using npm.api.DTO;
    using System.Collections.Generic;
    using Web.DAO;

    /// <summary>
    /// 課程
    /// </summary>
    public partial class LessonDAO : OrmBaseDAO<LessonDTO>
    {
        public IEnumerable<LessonDTO> GetRandom(int count)
        {
            var sql = $@"SELECT * FROM Lesson 
                ORDER BY RAND() LIMIT {count}";

            return this.conn.Query<LessonDTO>(sql);
        }

        public int UpdateVisitCount(int lessonID, int count)
        {
            return this.conn.Execute("UPDATE Lesson " +
                "SET VisitCount = VisitCount + @Count WHERE LessonID = @LessonID",
                new
                {
                    count,
                    lessonID
                });
        }

        public IEnumerable<TraceLessonModel> GetTraceSummery(int lessonID, int classRoomID)
        {
            return this.conn.Query<TraceLessonModel>($@"
                SELECT m.Name, ml.Progress, mhw.FinishTime, mhw.CorrectCount, mhw.TotalCount
                FROM MemberLesson ml INNER JOIN Member m ON ml.MemberID = m.MemberID
                LEFT JOIN MemberHomeWork mhw  ON ml.LessonID = mhw.LessonID AND ml.MemberID = mhw.MemberID AND ml.ClassRoomID = mhw.ClassRoomID
                WHERE ml.LessonID = {lessonID} AND ml.ClassRoomID = {classRoomID}");
        }
    }
}
