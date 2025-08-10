namespace npm.api.Models.Dto
{
    using System;

    /// <summary>
    /// 搜尋物件
    /// </summary>
    public partial class SearchDto
    {
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 連結路徑
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 影片長度
        /// </summary>
        public int VideoLength { get; set; }

        /// <summary>
        /// 觀看次數
        /// </summary>
        public int VisitCount { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
