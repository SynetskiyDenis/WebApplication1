using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Web;
using System.Net;

namespace WebApplication1.DbServices
{
    public class MyAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public MyAuthorizeAttribute()
        {
        }
          public void OnAuthorization(AuthorizationFilterContext filterContext)
          {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new UnauthorizedResult();
            }
            else
            {
                SqlConnection _connection = new SqlConnection("Server=DENIS-PC; Database=TestDBUsers; User ID=TestUser; Password=123456");

                _connection.Open();

                var txt = String.Format("Select id from Users where username='{0}' and islogined=1", filterContext.HttpContext.User.Identity.Name);

                SqlCommand cmd = new SqlCommand(txt, _connection);

                var res = cmd.ExecuteScalar();

                _connection.Close();

                if (res == null)
                {
                    filterContext.Result = new UnauthorizedResult();
                }
            }
          }
    }
}
