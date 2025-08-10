namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class VMemberRegisterStatisticsDTO
    {
        public DateTime Date { get; set; }

        public int TotalCount { get; set; }

        public int Type { get; set; }
    }
}
