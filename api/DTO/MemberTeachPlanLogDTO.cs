namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class MemberTeachPlanLogDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey]
        public int MemberTeachPlanLogID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int TeachPlanID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int? TeachPlanFileID { get; set; }

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
        public DateTime CreateTime { get; set; }
    }
}
