namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class MemberLessonChapterProgressDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey]
        public int MemberID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [PrimaryKey]
        public int LessonChapterID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Done { get; set; }
    }
}
