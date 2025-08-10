namespace Web.Filter
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;
    using Web.Core;
    using static Web.Core.Token;

    public class AuthFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                TokenType[] type = null;
                var skip = actionContext.ActionDescriptor.GetCustomAttributes<SkipAttribute>();
                if (skip != null && skip.Count > 0)
                {
                    return;
                }

                var auth = actionContext.ActionDescriptor.GetCustomAttributes<AuthAttribute>();
                if (auth != null && auth.Count > 0)
                {
                    type = auth[0].Type;
                }

                var queryString = actionContext.Request
                        .GetQueryNameValuePairs()
                        .ToDictionary(x => x.Key, x => x.Value);

                if (queryString.TryGetValue("t", out string t))
                {
                    if (!string.IsNullOrEmpty(t))
                    {
                        var token = Token.FromString(t);
                        if (token != null)
                        {
                            if (type == null)
                            {
                                return;
                            }
                            else if (type.Contains(token.Type))
                            {
                                return;
                            }
                        }
                    }
                }

                HttpResponseMessage result = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    RequestMessage = actionContext.Request
                };

                actionContext.Response = result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }

    public class SkipAttribute : System.Attribute
    {
    }

    public class AuthAttribute : System.Attribute
    {
        public TokenType[] Type { get; set; }

        public AuthAttribute(params TokenType[] type)
        {
            this.Type = type;
        }
    }
}
