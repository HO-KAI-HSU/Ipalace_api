namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class VLessonFinishedStatisticsDTO
    {
        public DateTime Date { get; set; }

        public int LessonID { get; set; }

        public int TotalCount { get; set; }
    }
}
