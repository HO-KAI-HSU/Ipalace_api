namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using Web.DAO;

    /// <summary>
    /// 後臺使用者
    /// </summary>
    public partial class UserDAO : OrmBaseDAO<UserDTO>
    {
        public UserDTO Login(string userName)
        {
            return conn.QueryFirstOrDefault<UserDTO>("SELECT  * FROM `User` WHERE Active = 1 and IsDeleted = 0 and UserName = @UserName", new { userName });
        }
    }
}
