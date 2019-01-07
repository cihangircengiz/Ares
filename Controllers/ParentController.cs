using Ares.EntityData.Model;
using Ares.Helpers;
using Ares.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ares.Controllers
{
    public class ParentController : Controller
    {
        // GET: Parent
        aresdbEntities dbContext = new aresdbEntities();
        [_SessionControl]
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            List<Parent> parentList = new List<Parent>();
            parentList = dbContext.Parents.Where(s => s.firmId == firmId).ToList();
            return View(parentList);
        }
        [_SessionControl]
        public ActionResult Create()
        {
            return View();
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Create(ParentCrudModel obj)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            Parent parent = new Parent();
            parent.name = obj.name;
            parent.phone = obj.phone;
            parent.tcNo = obj.tcNo;
            parent.firmId = firmId;
            parent.createdDate = DateTime.Now;
            parent.address = obj.address;
            parent.email = obj.email;
            dbContext.Parents.Add(parent);
            dbContext.SaveChanges();

            SysUser sysUser = new SysUser();
            sysUser.email = parent.email;
            sysUser.createdDate = DateTime.Now;
            sysUser.firmId = firmId;
            sysUser.name = parent.name;
            sysUser.type = AppSettingHelper.GetParentLoginSettingId();
            sysUser.typeId = parent.id;
            sysUser.username = obj.phone;
            sysUser.password = obj.password;
            dbContext.SysUsers.Add(sysUser);
            dbContext.SaveChanges();

            return Redirect("/Parent/Index");
        }
        [_SessionControl]
        public ActionResult Detail(int? id)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());

            if (id == 0 || id == null)
                return Redirect("/Parent/Index");
            Parent parent = new Parent();
            parent = dbContext.Parents.FirstOrDefault(s => s.id == id);
            if(parent==null)
                return Redirect("/Parent/Index");
            int type = AppSettingHelper.GetParentLoginSettingId();
            SysUser sysUser = dbContext.SysUsers.FirstOrDefault(s => s.typeId == parent.id && s.type==type && s.firmId==firmId);
            ParentCrudModel model = new ParentCrudModel();
            model.id = parent.id;
            model.name = parent.name;
            model.address = parent.address;
            model.email = parent.email;
            model.password = sysUser.password;
            model.username = sysUser.username;
            model.sysUserId = sysUser.id;
            model.phone = parent.phone;
            model.tcNo = parent.tcNo;

            List<Student> studentList = dbContext.Students.Where(s => s.firmId == firmId && s.parentId==parent.id).ToList();
            model.studentList = studentList;

            return View(model);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Detail(ParentCrudModel obj)
        {
            Parent parent = new Parent();
            parent = dbContext.Parents.FirstOrDefault(s => s.id == obj.id);
            List<Student> students = dbContext.Students.Where(s => s.parentId == parent.id).ToList();
            if (Request.Form["Delete"] != null)
            {
                /// delete all student
                dbContext.Students.RemoveRange(dbContext.Students.Where(s => s.parentId == obj.id));
                /// delete all related station
                foreach (var item in students)
                {
                    dbContext.Stations.RemoveRange(dbContext.Stations.Where(s => s.id == item.stationId));
                }
                //
                dbContext.Parents.Remove(parent);
                dbContext.SysUsers.Remove(dbContext.SysUsers.Where(s => s.typeId == parent.id).FirstOrDefault());
                dbContext.SaveChanges();
                return Redirect("/Parent/Index");
            }
            else { 
            parent.name = obj.name;
            parent.phone = obj.phone;
            parent.tcNo = obj.tcNo;
            parent.address = obj.address;
            parent.email = obj.email;
            dbContext.SaveChanges();

            SysUser sysUser = new SysUser();
            int type = AppSettingHelper.GetParentLoginSettingId();
            sysUser = dbContext.SysUsers.FirstOrDefault(s => s.id == obj.sysUserId && s.type==type);
            sysUser.name = obj.name;
            sysUser.email = obj.email;
            sysUser.username = obj.username;
            sysUser.password = obj.password;
            dbContext.SaveChanges();
            return Redirect("/Parent/Index");
            }
        }
    }
}