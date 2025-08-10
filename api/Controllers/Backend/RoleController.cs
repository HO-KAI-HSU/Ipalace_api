namespace npm.api.API.Controllers
{
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Web.Http;
    using Web;

    public partial class BackendController : BaseController
    {
        [HttpGet]
        public ResponseDTO GetRoles()
        {
            using (var dao = GetDAO<RoleDAO>())
            {
                try
                {
                    var roles = dao.GetAll();
                    return OK(roles);
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpGet]
        public ResponseDTO GetRole(int id)
        {
            using (var dao = GetDAO<RoleDAO>())
            {
                try
                {
                    var user = dao.Get(id);
                    return OK(user);
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpPost]
        public ResponseDTO CreateRole(RoleDTO model)
        {
            using (var dao = GetDAO<RoleDAO>())
            {
                return OK(dao.Insert(model));
            }
        }

        [HttpPut]
        public ResponseDTO UpdateRole(int id, RoleDTO model)
        {
            using (var dao = GetDAO<RoleDAO>())
            {
                var dto = dao.Get(id);
                dto.RoleName = model.RoleName;
                dto.Function = model.Function;
                return OK(dao.Update(dto));
            }
        }

        [HttpDelete]
        public ResponseDTO DeleteRole(int id)
        {
            using (var dao = GetDAO<RoleDAO>())
            {
                var dto = dao.Get(id);
                return OK(dao.Delete(id));
            }
        }
    }
}