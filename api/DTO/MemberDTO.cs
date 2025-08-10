namespace npm.api.DTO
{
    using com.leslie.Core.ORM.MySql;
    using System;

    /// <summary>
    ///
    /// </summary>
    [Table]
    public partial class MemberDTO
    {
        /// <summary>
        ///
        /// </summary>
        [PrimaryKey(AutoIncrement = true)]
        public int MemberID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBIgnoreWhenList]
        [DBUpdateIgnore]
        public string Password { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBIgnoreWhenList]
        public string OpenID { get; set; }

        /// <summary>
        ///
        /// </summary>
        public MemberType Type { get; set; }

        /// <summary>
        ///
        /// </summary>
        public MemberStatus Status { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string School { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DBNow]
        public DateTime CreateTime { get; set; }

        [DBIgnoreWhenList]
        public DateTime? LastUpdatePasswordTime { get; set; }

        public enum MemberStatus : byte
        {
            Normal = 0,
            Ban = 1,
            Deleted = 2
        }

        public enum MemberType : byte
        {
            User = 0,
            Teacher = 1,
            Student = 2
        }
    }
}
