namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class VLessonChapterHistoryDTO
    {
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LessonName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Brief { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ImageUrl { get; set; }

        public int VideoLength { get; set; }

        public int MemberID { get; set; }

        public int LessonChapterID { get; set; }

        public int LessonSubjectID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LessonSubjectName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ChapterName { get; set; }

        public int ChapterVideoLength { get; set; }

        /// <summary>
        ///
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
