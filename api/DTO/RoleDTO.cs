namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 後台使用者群組
    /// </summary>
    [Table]
    public partial class RoleDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int RoleID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 啟用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 功能清單，逗號(,)分隔
        /// </summary>
        public string Function { get; set; }

        [DBIgnore]
        public List<string> Functions
        {
            get => Function == null ?
                new List<string>() :
                Function.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
