namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class VLessonStatisticsDTO
    {
        public DateTime Date { get; set; }

        public int LessonID { get; set; }

        public int Type { get; set; }

        public int TotalCount { get; set; }

        public float TotalTime { get; set; }
    }
}
