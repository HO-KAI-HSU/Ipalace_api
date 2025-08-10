namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;

    /// <summary>
    /// 課程章節
    /// </summary>
    [Table]
    public partial class LessonChapterDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int LessonChapterID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ChapterName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int VideoLength { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public short Sort { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LessonSubjectID { get; set; }
    }
}
