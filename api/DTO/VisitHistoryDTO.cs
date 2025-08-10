namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class VisitHistoryDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int VisitHistoryID { get; set; }

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
        [DBNow(true)]
        public DateTime CreateTime { get; set; }
    }
}
