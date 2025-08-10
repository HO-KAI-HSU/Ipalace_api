namespace npm.api.DAO
{
    using npm.api.DTO;
    using Web.DAO;

    /// <summary>
    ///
    /// </summary>
    public partial class MemberLessonChapterProgressDAO : OrmBaseDAO<MemberLessonChapterProgressDTO>
    {
        public MemberLessonChapterProgressDTO Get(int memberID, int lessonChapterID)
        {
            return this.Get($"MemberID = @MemberID AND LessonChapterID = @LessonChapterID",
                new
                {
                    memberID,
                    lessonChapterID
                });
        }
    }
}
