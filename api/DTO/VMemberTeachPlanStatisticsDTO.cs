namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class VMemberTeachPlanStatisticsDTO
    {
        public DateTime Date { get; set; }

        public int MemberID { get; set; }

        public int Type { get; set; }

        public int TotalCount { get; set; }
    }
}
