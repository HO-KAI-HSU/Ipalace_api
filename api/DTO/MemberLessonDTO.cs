namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    /// 我的班級
    /// </summary>
    [Table]
    public partial class MemberLessonDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int MemberLessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int MemberID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int ClassRoomID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime? FinishTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }
    }
}
