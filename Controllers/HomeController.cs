using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.EntityData;
using Ares.EntityData.Model;
using System.Web.Security;
using Ares.Models;
using Ares.Helpers;

namespace Ares.Controllers
{
    public class HomeController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: Home
        [_SessionControl]
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());

            List<Student> studentList = dbContext.Students.Where(s => s.firmId == firmId).ToList();
            int studentCount = studentList.Count;
            List<Student> lastFiveStudent = studentList.OrderByDescending(p => p.firmId==firmId).Take(5).ToList();

            List<Bus> busList = dbContext.Buses.Where(e => e.firmId == firmId).ToList();
            int busCount = busList.Count;
            List<Bus> lastFiveBus = busList.OrderByDescending(c => c.id).Take(5).ToList();

            List<Route> routeList = dbContext.Routes.Where(e => e.firmId == firmId).ToList();
            int routeCount = routeList.Count;
            List<Route> lastFiveRoute = routeList.OrderByDescending(c => c.id).Take(5).ToList();

            List<School> schoolList = dbContext.Schools.Where(b => b.firmId == firmId).ToList();
            int schoolCount = schoolList.Count;
            List<School> lastFiveSchool = schoolList.OrderByDescending(c => c.id).Take(5).ToList();

            List<Parent> parentList = new List<Parent>();
            parentList = dbContext.Parents.Where(p => p.firmId == firmId).ToList();
            int parentCount = parentList.Count;

            List<Station> stationList = dbContext.Stations.Where(a => a.firmId == firmId).ToList();
            int stationCount = stationList.Count;

            
            DashboardModel model = new DashboardModel();
            model.studentCount = studentCount;
            model.parentCount = parentCount;
            model.studentList = studentCount;
            model.busCount = busCount;
            model.stationCount = stationCount;
            model.schoolCount = schoolCount;
            model.lastFiveStudent =lastFiveStudent;
            model.lastFiveBus = lastFiveBus;
            model.lastFiveRoute = lastFiveRoute;
            model.lastFiveSchool = lastFiveSchool;
            model.routeCount = routeCount;

            return View(model);
        }
        public ActionResult Example()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddFirmLogin(FirmLogin obj)
        {
            obj.firmId = 1;
            obj.createdDate = DateTime.Now;
            dbContext.FirmLogins.Add(obj);
            dbContext.SaveChanges();
            return Redirect("/Home/Index");
        }
    }
}