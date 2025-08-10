namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;

    /// <summary>
    /// 教學資源專區分類
    /// </summary>
    [Table]
    public partial class TeachPlanCategoryDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int TeachPlanCategoryID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public byte Sort { get; set; }
    }
}
