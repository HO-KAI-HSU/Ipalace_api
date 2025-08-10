namespace npm.api.Models.Dto
{
    /// <summary>
    /// 會員物件
    /// </summary>
    public class MemberDto
    {
        /// <summary>
        /// 會員名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 學校
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// 班別
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 金鑰
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 密碼是否已過期
        /// </summary>
        public bool IsExpiredPwd { get; set; }
    }
}
