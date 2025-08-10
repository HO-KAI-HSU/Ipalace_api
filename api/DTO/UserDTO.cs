namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    /// 後臺使用者
    /// </summary>
    [Table]
    public partial class UserDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int UserID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int RoleID { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [DBNow]
        [DBUpdateIgnore]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        public DateTime LastUpdatePasswordTime { get; set; }

        public int LoginErrorCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsDefaultPassword { get; set; }

        /// <summary>
        ///
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
