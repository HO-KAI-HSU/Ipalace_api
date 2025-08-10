namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 教學資源專區
    /// </summary>
    [Table]
    public partial class TeachPlanDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int TeachPlanID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int TeachPlanCategoryID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// 教學領域
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 外部連結
        /// </summary>
        public string ExternalLink { get; set; }

        /// <summary>
        /// 適用對象
        /// </summary>
        [DBIgnoreWhenList]
        public string Target { get; set; }

        /// <summary>
        /// 欣賞影片
        /// </summary>
        public string VideoName { get; set; }

        /// <summary>
        /// 教學目標
        /// </summary>
        [DBIgnoreWhenList]
        public string Goal { get; set; }

        /// <summary>
        /// 教案下載
        /// </summary>
        [DBIgnoreWhenList]
        public string TeachPlanUrl { get; set; }

        /// <summary>
        /// 教案原始檔名
        /// </summary>
        [DBIgnoreWhenList]
        public string TeachPlanFileName { get; set; }

        /// <summary>
        /// 學習單下載
        /// </summary>
        [DBIgnoreWhenList]
        public string LearnSheetUrl { get; set; }

        /// <summary>
        /// 學習單原始檔名
        /// </summary>
        [DBIgnoreWhenList]
        public string LearnSheetFileName { get; set; }

        /// <summary>
        /// 教學示範影片觀看
        /// </summary>
        [DBIgnoreWhenList]
        public string VideoUrl { get; set; }

        /// <summary>
        /// 教學示範影片原始檔名
        /// </summary>
        [DBIgnoreWhenList]
        public string VideoFileName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        public int? LessonID { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }

        [DBIgnore]
        public IEnumerable<TeachPlanFileDTO> TeachPlanFiles { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int VisitCount { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public int DownloadCount { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }

    public class TeachPlanFile
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }
}
