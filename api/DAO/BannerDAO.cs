namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System.Collections.Generic;
    using Web.DAO;

    /// <summary>
    ///
    /// </summary>
    public partial class BannerDAO : OrmBaseDAO<BannerDTO>
    {
        public int UpdateSort(IEnumerable<BannerDTO> data)
        {
            return conn.Execute($"UPDATE {TableName} " +
                $"SET Sort = @Sort " +
                $"WHERE BannerID = @BannerID",
                data);
        }
    }
}
