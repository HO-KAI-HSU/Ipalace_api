namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    /// 後臺使用者
    /// </summary>
    [Table]
    public partial class UserPasswordLogDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int UserPasswordLogID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [DBNow]
        [DBUpdateIgnore]
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Comment { get; set; }
    }
}
