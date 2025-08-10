namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System;
    using Web.DAO;

    /// <summary>
    ///
    /// </summary>
    public partial class MemberDAO : OrmBaseDAO<MemberDTO>
    {
        public MemberDTO GetByEmail(string email)
        {
            return this.Get("Email = @Email", new { email });
        }

        public int UpdatePassword(int memberID, string password)
        {
            return this.conn.Execute("UPDATE Member " +
                "SET Password = @Password, LastUpdatePasswordTime = @LastUpdatePasswordTime " +
                "WHERE MemberID = @MemberID",
                new
                {
                    memberID,
                    password,
                    LastUpdatePasswordTime = DateTime.Now
                });
        }
    }
}
