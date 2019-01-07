using Ares.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.EntityData.Model;
using Ares.Models;

namespace Ares.Controllers
{
    public class AccountController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: Account
        [_SessionControl]
        public ActionResult Index()
        {
            return View();
        }
        [_SessionControl]
        public ActionResult Detail()
        {
            LoginInfo loginInfo = new LoginInfo();
            int id = 0;
            if (String.IsNullOrEmpty(loginInfo.GetUserId()))
                return RedirectToAction("Login", "Login");
            id = Convert.ToInt32(loginInfo.GetUserId());
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            FirmLogin user = dbContext.FirmLogins.FirstOrDefault(f => f.id == id && f.firmId == firmId);
            return View(user);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Detail(FirmLogin user)
        {
            FirmLogin userInfo = dbContext.FirmLogins.FirstOrDefault(f => f.id == user.id);
            userInfo.email = user.email;
            userInfo.password = user.password;
            userInfo.username = user.username;
            userInfo.name = user.name;
            dbContext.SaveChanges();
            return Redirect("/Account/Detail");
        }
    }
}