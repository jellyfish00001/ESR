using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp.AspNetCore.Mvc;
namespace ERS.Controllers
{
    [Authorize]
    [Route("api")]
    public class BaseController : AbpController
    {
        public IMemoryCache _memoryCache { get; set; }
        public string email
        {
            get
            {
                return _memoryCache.Get("email")?.ToString();
                //return this.HttpContext.Items["email"]?.ToString();
                //string token = this.HttpContext.Request.Headers["Authorization"];
                //Console.WriteLine("Authorization:");
                //Console.WriteLine(token);
                //var handler = new JwtSecurityTokenHandler();
                //string email = "";
                //if (handler.CanReadToken(token))
                //{
                //    var jwtToken = handler.ReadJwtToken(token);
                //    Console.WriteLine("JWT Claims:");
                //    foreach (var claim in jwtToken.Claims)
                //    {
                //        Console.WriteLine($"{claim.Type}: {claim.Value}");
                //        if (claim.Type == "unique_name")
                //        {
                //            email = claim.Value;
                //        }
                //    }
                //    // 使用 LINQ 获取 "preferred_username" 的值
                //    email = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
                //}
                //return email;
            }
        }
        public string userId
        {
            get
            {
               return _memoryCache.Get("userId")?.ToString();
                //return this.HttpContext.Items["userId"]?.ToString();
                //if (!string.IsNullOrEmpty(email))
                //{
                //    var emp = EmployeeInfoRepository.QueryByEmail(email);
                //    return emp.Result.emplid;
                //}
                //else
                //{
                //    return "";
                //}
                //var emp = EmployeeInfoRepository.QueryByEmail(email);
                //if (this.HttpContext == null || this.HttpContext.User == null || this.HttpContext.User.Claims == null
                //    || this.HttpContext.User.Claims.Count() == 0)
                //{
                //    return "N/A";
                //}
                //if (this.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub") == null)
                //{
                //    return this.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value.ToUpper();
                //}
                //return this.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub").Value.ToUpper();
            }
        }

        public string userName
        {
            get
            {
                return _memoryCache.Get("userName")?.ToString();
                //return this.HttpContext.Items["userName"]?.ToString();
                //if (!string.IsNullOrEmpty(email))
                //{
                //    var emp = EmployeeInfoRepository.QueryByEmail(email);
                //    return emp.Result.name_a;
                //}
                //else
                //{
                //    return "";
                //}
            }
        }
        public string token
        {
            get
            {
                return this.HttpContext.Request.Headers["Authorization"];
            }
        }
    }
}
