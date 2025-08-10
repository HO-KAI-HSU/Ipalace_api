namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    /// 教學資源專區
    /// </summary>
    [Table]
    public partial class TeachPlanFileDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int TeachPlanFileID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int TeachPlanID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        public string Url { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}
