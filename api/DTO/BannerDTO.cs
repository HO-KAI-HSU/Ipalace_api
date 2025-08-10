namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class BannerDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int BannerID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string BannerName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int BannerType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string TargetUrl { get; set; }
    }
}
