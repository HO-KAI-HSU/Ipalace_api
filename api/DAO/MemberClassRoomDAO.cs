namespace npm.api.DAO
{
    using Dapper;
    using npm.api.DTO;
    using System.Collections.Generic;
    using System.Linq;
    using Web.DAO;

    /// <summary>
    /// 我的班級
    /// </summary>
    public partial class MemberClassRoomDAO : OrmBaseDAO<MemberClassRoomDTO>
    {
        public IEnumerable<ClassRoomDTO> GetClassRoomByMember(int memberID)
        {
            return this.conn.Query<ClassRoomDTO>($"SELECT * FROM ClassRoom " +
                $"WHERE ClassRoomID IN (SELECT ClassRoomID FROM MemberClassRoom WHERE MemberID = {memberID})");
        }

        public Dictionary<int, int> GetFinish(IEnumerable<int> classRoomIDs)
        {
            return this.conn.Query(@"SELECT ClassRoomID, count(*) AS `Count` FROM MemberClassRoom 
                WHERE Progress = 100 AND ClassRoomID IN @ClassRoomIDs 
                GROUP BY ClassRoomID",
                new
                {
                    ClassRoomIDs = classRoomIDs.ToArray()
                })
                .ToDictionary(x => (int)x.ClassRoomID, x => (int)x.Count);
        }

        public IEnumerable<MemberClassRoomSummeryDTO> GetSummery(IEnumerable<int> classRoomID)
        {
            return this.conn.Query<MemberClassRoomSummeryDTO>("SELECT ClassRoomID, COUNT(*) AS Count FROM MemberClassRoom " +
                "WHERE ClassRoomID IN @ClassRoomID GROUP BY ClassRoomID",
                new
                {
                    ClassRoomID = classRoomID.ToArray()
                });
        }
    }
}
