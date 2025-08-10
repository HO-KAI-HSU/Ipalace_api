namespace npm.api.API.Controllers
{
    using System.Web;
    using System.Web.Http;
    using Web;
    using Web.Core;
    using Web.DAO;

    public class BaseController : ApiController
    {
        protected ResponseDTO OK(object result = null, string contentType = null)
        {
            var ret = new ResponseDTO()
            {
                Success = true,
                Result = result,
            };

            return ret;
        }

        protected ResponseDTO Error(Status errorCode, object errorContent = null)
        {
            var ret = new ResponseDTO()
            {
                Success = false,
                Status = errorCode,
                Result = errorContent
            };
            return ret;
        }

        protected T GetDAO<T>()
            where T : BaseDAO, new()
        {
            return new T();
        }

        protected T GetDAO<T>(BaseDAO dao)
            where T : BaseDAO, new()
        {
            var t = new T();
            t.Combine(dao);
            return t;
        }

        private Token token;

        protected Token Token
        {
            get
            {
                if (token == null)
                {
                    // return Token.FromClaimsPrincipal(HttpContext.User);
                    string value = HttpContext.Current.Request.QueryString["t"];
                    token = Token.FromString(value);
                }

                return token;
            }

            set
            {
                token = value;
            }
        }

        protected T Trim<T>(T source)
        {
            var t = typeof(T);
            foreach (var p in t.GetProperties())
            {
                if (p.PropertyType == typeof(string))
                {
                    var value = p.GetValue(source)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        p.SetValue(source, value.Trim());
                    }
                }
            }

            return source;
        }
    }

    public class ResponseDTO<T>
    {
        public Status Status { get; set; }

        public T Result { get; set; }

        public bool Success { get; set; }
    }

    public class ResponseDTO : ResponseDTO<object>
    {
    }
}