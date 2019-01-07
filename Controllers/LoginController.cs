using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.EntityData.Model;
using System.Web.Security;
using Ares.Models;
using Ares.Helpers;

namespace Ares.Controllers
{
    public class LoginController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {
            LoginInfo loginInfo = new LoginInfo();
            if (String.IsNullOrEmpty(loginInfo.GetUserId()))
            {
                return View();
            }
            return Redirect("/Home/Index");
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnurl)
        {
            if (ModelState.IsValid)
            {
                FirmLogin firmLogin = new FirmLogin();
                firmLogin = dbContext.FirmLogins.Where(f => f.email == model.email && f.password == model.password).FirstOrDefault();

                if (firmLogin != null)
                {
                    AddCookie(firmLogin.id.ToString(), firmLogin.firmId.ToString());
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is wrong!");
                }
            }
            return View(model);
        }
        [_SessionControl]
        public ActionResult LogOff()
        {
            DeleteCookie();
            return RedirectToAction("Login", "Login");
        }

        public void AddCookie(string firmLoginId, string firmLoginFirmId)
        {
            HttpCookie userId = new HttpCookie("userId", firmLoginId);
            userId.Expires = DateTime.Now.AddYears(1);
            HttpContext.Response.Cookies.Add(userId);

            HttpCookie firmId = new HttpCookie("firmId", firmLoginFirmId);
            firmId.Expires = DateTime.Now.AddYears(1);
            HttpContext.Response.Cookies.Add(firmId);
        }
        public void DeleteCookie()
        {
            HttpCookie userId = new HttpCookie("userId");
            userId.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Response.Cookies.Add(userId);

            HttpCookie firmId = new HttpCookie("firmId");
            userId.Expires = DateTime.Now.AddYears(-1);
            HttpContext.Response.Cookies.Add(firmId);
        }

    }
}