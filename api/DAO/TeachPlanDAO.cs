namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using Web.DAO;

    /// <summary>
    /// 教學資源專區
    /// </summary>
    public partial class TeachPlanDAO : OrmBaseDAO<TeachPlanDTO>
    {
        public int UpdateVisitCount(int teachPlanID, int count)
        {
            return this.conn.Execute("UPDATE TeachPlan SET VisitCount = VisitCount + @Count WHERE TeachPlanID = @TeachPlanID", new
            {
                count,
                teachPlanID
            });
        }

        public int UpdateDownloadCount(int teachPlanID, int count)
        {
            return this.conn.Execute("UPDATE TeachPlan SET DownloadCount = DownloadCount + @Count WHERE TeachPlanID = @TeachPlanID", new
            {
                count,
                teachPlanID
            });
        }
    }
}
