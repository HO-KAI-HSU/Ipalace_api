namespace npm.api.Models.Dto
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 搜尋物件
    /// </summary>
    public class AudioCaptchaRequestDto
    {
        [Required]
        /// <summary>
        /// 開始時間
        /// </summary>
        public string Secret { get; set; }
    }
}
