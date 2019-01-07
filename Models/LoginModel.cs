using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ares.Controllers;

namespace Ares.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        [Display(Name = "EMail")]
        public string email { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
    class _SessionControlAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string cookieValue = string.Empty;
            if (HttpContext.Current.Request.Cookies["userId"] != null)
                cookieValue= HttpContext.Current.Request.Cookies["userId"].Value;
            if (String.IsNullOrEmpty(cookieValue))
            {
                if (!HttpContext.Current.Response.IsRequestBeingRedirected)
                    filterContext.HttpContext.Response.Redirect("/Login/Login");
            }
        }
    }
}