namespace npm.api.Models.Dto
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 搜尋物件
    /// </summary>
    public class DateRequestDto
    {
        [Required]
        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime StartDate { get; set; }

        [Required]
        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}