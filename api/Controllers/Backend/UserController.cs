namespace npm.api.API.Controllers
{
    using npm.api.API.Helper;
    using npm.api.API.Models.Backend;
    using npm.api.DAO;
    using npm.api.DTO;
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Web.Http;
    using Web;
    using Web.Configs;
    using Web.Core;
    using Web.Filter;

    public partial class BackendController : BaseController
    {
        private string EncodePassword(string password)
        {
            MD5 md5 = MD5.Create();
            return string
                .Join("", md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "ipalace.npm.edu.tw"))
                .Select(x => x.ToString("X2")));
        }

        [HttpPost]
        [Skip]
        public ResponseDTO Login([FromBody] UserDTO request)
        {
            try
            {
                var userName = Config.Instance.Backend.UserName;
                var password = Config.Instance.Backend.Password;
                if (request.UserName == userName && request.Password == password)
                {
                    return OK(new
                    {
                        name = "系統管理者",
                        roleId = 1,
                        role = "Account,AccountCreate,AccountDelete,TechPlan,Lesson,News,User,UserEdit,Document,Product,Member,MemberEdit,Brand,HomeBrick,Banner,Category,Coupon,Event,Store,SystemConfig,PointConfig,Order,Css,SendPoint,PointSendLog,ExpensiveNotify,Role".Split(','),
                        t = new Token()
                        {
                            AccountID = 0,
                            Type = Token.TokenType.Admin
                        }.Generate()
                    });
                }
            }
            catch (Exception e)
            {
                return Error(Status.GeneralError, e.Message);
            }

            using (var dao = GetDAO<UserDAO>())
            using (var rdao = GetDAO<RoleDAO>(dao))
            {
                try
                {
                    var isExpiredPwd = false;
                    var encryptPassword = EncodePassword(request.Password);
                    var user = dao.Login(request.UserName);
                    if (user == null)
                    {
                        return Error(Status.LoginFail);
                    }

                    if (user.LoginErrorCount >= 3 && user.LastLoginTime.AddMinutes(15) > DateTime.Now)
                    {
                        return Error(Status.AccountLocked);
                    }

                    if (!user.Password.Equals(encryptPassword))
                    {
                        if (user.LoginErrorCount >= 3)
                        {
                            user.LoginErrorCount = 0;
                        }

                        user.LoginErrorCount = user.LoginErrorCount + 1;
                        if (user.LoginErrorCount >= 3 && user.LastLoginTime.AddMinutes(15) > DateTime.Now)
                        {
                            dao.Update(user);
                            return Error(Status.AccountLocked);
                        }

                        user.LastLoginTime = DateTime.Now;
                        dao.Update(user);

                        return Error(Status.LoginFail);
                    }

                    if (user.LastUpdatePasswordTime == null ||
                        user.LastUpdatePasswordTime.AddMonths(3) < DateTime.Now)
                    {
                        isExpiredPwd = true;
                    }

                    var role = rdao.Get(user.RoleID);
                    user.LastLoginTime = DateTime.Now;
                    user.LoginErrorCount = 0;
                    dao.Update(user);

                    return OK(new
                    {
                        name = user.Name,
                        role = role.Function,
                        roleId = user.RoleID,
                        t = new Token()
                        {
                            AccountID = user.UserID,
                            Name = user.Name,
                            Type = Token.TokenType.Admin
                        }.Generate(),
                        isExpiredPwd = isExpiredPwd,
                        isDefaultPwd = user.IsDefaultPassword
                    });
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpPost]
        [Skip]
        public ResponseDTO ChangePassword([FromBody] ChangePasswordModel request)
        {
            var userName = request.UserName;
            var oldPassword = request.OldPassword;
            var newPassword = request.NewPassword;
            using (var dao = GetDAO<UserDAO>())
            using (var userPasswordLogDao = GetDAO<UserPasswordLogDAO>(dao))     
            {
                try    
                {
                    var user = dao.Get("UserName = @UserName", new { userName });
                    var userPasswordHistorys = userPasswordLogDao.GetAll("UserID = @UserID", new { user.UserID })
                        .OrderByDescending(x => x.CreateTime)
                        .Select(x => x.NewPassword)
                        .Take(3);

                    if (!CommonHelper.CheckPasswordComplexity(newPassword))
                    {
                        return Error(Status.LowPasswordComplexity);
                    }

                    if (user.Password != EncodePassword(oldPassword))
                    {
                        return Error(Status.GeneralError);
                    }

                    if (user.Password == EncodePassword(newPassword) ||
                        userPasswordHistorys.Any(x => x == EncodePassword(newPassword)))
                    {
                        return Error(Status.DuplicateOldPassword);
                    }

                    CreateUserPasswordLog(user.UserID, EncodePassword(oldPassword), EncodePassword(newPassword), userPasswordLogDao);

                    user.Password = EncodePassword(newPassword);
                    user.IsDefaultPassword = false;
                    user.LastUpdatePasswordTime = DateTime.Now;
                    return OK(dao.Update(user));
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpGet]
        public ResponseDTO GetAccounts()
        {
            var t = Token;
            using (var dao = GetDAO<UserDAO>())
            {
                try
                {
                    if (t.AccountID > 0)
                    {
                        var userID = t.AccountID;
                        var user = dao.Get(userID);
                        if (user.RoleID < 2)
                        {
                            return OK(dao.GetAll("IsDeleted = 0"));
                        }
                        else
                        {
                            return OK(dao.GetAll("IsDeleted = 0 and UserID = " + userID));
                        }
                    }
                    else
                    {
                        return OK(dao.GetAll());
                    }
                }
                catch (Exception)
                {
                    return Error(Status.NotFound);
                }
            }
        }

        [HttpGet]
        public ResponseDTO GetAccount(int id)
        {
            using (var dao = GetDAO<UserDAO>())
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
        public ResponseDTO CreateAccount(UserDTO model)
        {
            using (var dao = GetDAO<UserDAO>())
            {
                var user = dao.Get("UserName = @UserName and IsDeleted = 0", new { model.UserName });
                if (user != null)
                {
                    return Error(Status.Duplicate);
                }

                model.Password = EncodePassword(model.Password);
                model.LastUpdatePasswordTime = DateTime.Now;
                return OK(dao.Insert(model));
            }
        }

        [HttpPut]
        public ResponseDTO UpdateAccount(int id, UserDTO model)
        {
            var t = Token;
            using (var dao = GetDAO<UserDAO>())
            using (var userPasswordLogDao = GetDAO<UserPasswordLogDAO>(dao))
            {
                var actionUserID = t.AccountID;
                var dto = dao.Get(id);
                var userPasswordHistorys = userPasswordLogDao.GetAll("UserID = @UserID", new { UserID = id })
                    .OrderByDescending(x => x.CreateTime)
                    .Select(x => x.NewPassword)
                    .Take(3);

                if (id == actionUserID && dto.Password != model.Password && !CommonHelper.CheckPasswordComplexity(model.Password))
                {
                    return Error(Status.LowPasswordComplexity);
                }

                if (dto.Password == EncodePassword(model.Password) || userPasswordHistorys.Any(x => x == EncodePassword(model.Password)))
                {
                    return Error(Status.DuplicateOldPassword);
                }

                if (id == actionUserID)
                {
                    dto.IsDefaultPassword = false;
                    CreateUserPasswordLog(id, dto.Password, EncodePassword(model.Password), userPasswordLogDao);
                }
                else
                {
                    dto.IsDefaultPassword = model.Password != dto.Password ? true : false;
                }

                dto.UserName = model.UserName;
                dto.Name = model.Name;
                dto.Email = model.Email;
                dto.Active = model.Active;
                dto.Password = model.Password != dto.Password ? EncodePassword(model.Password) : dto.Password;
                dto.RoleID = model.RoleID;
                dto.LastUpdatePasswordTime = DateTime.Now;
                return OK(dao.Update(dto));
            }
        }

        [HttpDelete]
        public ResponseDTO DeleteAccount(int id)
        {
            using (var dao = GetDAO<UserDAO>())
            {
                var model = dao.Get(id);
                model.IsDeleted = true;
                return OK(dao.Update(model));
            }
        }

        private void CreateUserPasswordLog(int userId, string oldPwd, string newPwd, UserPasswordLogDAO userPasswordLogDao)
        {
            var userPwdLog = new UserPasswordLogDTO()
            {
                UserID = userId,
                OldPassword = oldPwd,
                NewPassword = newPwd,
            };
            userPasswordLogDao.Insert(userPwdLog);
        }
    }
}