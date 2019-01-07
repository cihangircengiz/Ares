using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.EntityData.Model;
using Ares.Helpers;
using Ares.Models;

namespace Ares.Controllers
{
    public class BusController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: Bus
        [_SessionControl]
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            List<Bus> busList = new List<Bus>();
            busList = dbContext.Buses.Where(b => b.firmId == firmId).ToList();
            return View(busList);
        }
        [_SessionControl]
        public ActionResult Create()
        {
            return View();
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Create(BusCrudModel obj)
        {
            LoginInfo loginInfo = new LoginInfo();
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login"); 
            int firmId = Convert.ToInt32(loginInfo.GetFirmId());
            Bus bus = new Bus();
            bus.plate = obj.plate;
            bus.model = obj.model;
            bus.brand = obj.brand;
            bus.authorizePersonName = obj.authorizePersonName;
            bus.authorizePersonMail = obj.authorizePersonMail;
            bus.authorizePersonPhone = obj.authorizePersonPhone;
            bus.firmId = firmId;
            bus.createdDate = DateTime.Now;
            bus.maxSeatCount = obj.maxSeatCount;
            dbContext.Buses.Add(bus);
            dbContext.SaveChanges();

            SysUser sysUser = new SysUser();
            sysUser.email = bus.authorizePersonMail;
            sysUser.createdDate = DateTime.Now;
            sysUser.firmId = firmId;
            sysUser.name = bus.authorizePersonName;
            sysUser.type = AppSettingHelper.GetBusLoginSettingId();
            sysUser.typeId = bus.id;
            sysUser.username = obj.username;
            sysUser.password = obj.password;
            dbContext.SysUsers.Add(sysUser);
            dbContext.SaveChanges();

            return Redirect("/Bus/Index");
        }
        [_SessionControl]
        public ActionResult Detail(int? id)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = Convert.ToInt32(loginInfo.GetFirmId());
            if (id == 0 || id == null)
                return Redirect("/Bus/Index");
            Bus bus = new Bus();
            bus = dbContext.Buses.FirstOrDefault(b => b.id == id && b.firmId == firmId);
            if (bus == null)
                return Redirect("/Bus/Index");
            int type = AppSettingHelper.GetBusLoginSettingId();
            SysUser sysUser = dbContext.SysUsers.FirstOrDefault(s => s.typeId == bus.id && s.type==type && s.firmId==firmId);
            BusCrudModel model = new BusCrudModel();
            model.busRoutes = dbContext.Routes.Where(s => s.busId == bus.id && s.firmId == firmId).ToList();
            model.id = bus.id;
            model.plate = bus.plate;
            model.model = bus.model;
            model.brand = bus.brand;
            model.authorizePersonName = bus.authorizePersonName;
            model.authorizePersonMail = bus.authorizePersonMail;
            model.authorizePersonPhone = bus.authorizePersonPhone;
            model.firmId = bus.firmId;
            model.createdDate = DateTime.Now;
            model.maxSeatCount = bus.maxSeatCount;
            model.password = sysUser.password;
            model.username = sysUser.username;
            model.sysUserId = sysUser.id;
            return View(model);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Detail(BusCrudModel obj)
        {
            Bus bus = new Bus();
            bus = dbContext.Buses.FirstOrDefault(s => s.id == obj.id);
            if (Request.Form["Delete"] != null)
            {
                SysUser sysUser = dbContext.SysUsers.FirstOrDefault(s => s.typeId == obj.id);
                dbContext.SysUsers.Remove(sysUser);
                dbContext.Buses.Remove(bus);
                dbContext.SaveChanges();
            }
            else { 
                bus.plate = obj.plate;
                bus.model = obj.model;
                bus.brand = obj.brand;
                bus.authorizePersonName = obj.authorizePersonName;
                bus.authorizePersonMail = obj.authorizePersonMail;
                bus.authorizePersonPhone = obj.authorizePersonPhone;
                bus.maxSeatCount = obj.maxSeatCount;
                dbContext.SaveChanges();

                SysUser sysUser = new SysUser();
                int type = AppSettingHelper.GetBusLoginSettingId();
                sysUser = dbContext.SysUsers.FirstOrDefault(s => s.id == obj.sysUserId && s.type == type);
                sysUser.name = obj.authorizePersonName;
                sysUser.email = obj.authorizePersonMail;
                sysUser.username = obj.username;
                sysUser.password = obj.password;
                dbContext.SaveChanges();
            }
            return Redirect("/Bus/Index");
        }
    }
}