namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class LessonCategoryDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int LessonCategoryID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string CategoryName { get; set; }
    }
}
