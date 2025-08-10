namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class MemberHomeWorkDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int MemberHomeWorkID { get; set; }

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
        [DBJson]
        public IEnumerable<int> Answers { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int CorrectCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int TotalCount { get; set; }

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
