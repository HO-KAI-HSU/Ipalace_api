namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class MemberLessonLogDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey]
        public int MemberLessonLogID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LessonChapterID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int MemberID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ClientIp { get; set; }

        /// <summary>
        ///
        /// </summary>
        public float? Time { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
