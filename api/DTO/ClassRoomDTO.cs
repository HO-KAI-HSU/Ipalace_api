namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 班級
    /// </summary>
    [Table]
    public partial class ClassRoomDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int ClassRoomID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ClassRoomName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LessonTitle { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ClassRoomStatus Status { get; set; } = ClassRoomStatus.Created;

        /// <summary>
        ///
        /// </summary>
        public bool HasHomeWork { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBJson]
        public IEnumerable<int> HomeWorks { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBJson]
        public IEnumerable<int> Lessons { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Code { get; set; }

        public int TeacherMemberID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }

        public enum ClassRoomStatus : byte
        {
            Created = 1,
            Temp = 0,
            Deleted = 2,
        }
    }
}
