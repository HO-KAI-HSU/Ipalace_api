namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class MemberLessonChapterFinishedLogDTO
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
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [PrimaryKey]
        public int LessonChapterID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }
    }
}
