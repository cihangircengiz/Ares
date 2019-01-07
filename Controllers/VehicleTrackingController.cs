using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.EntityData.Model;
using Ares.Helpers;
using Ares.Models;
using Newtonsoft.Json;

namespace Ares.Controllers
{
    public class VehicleTrackingController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: VehicleTracking
        [_SessionControl]
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            Firm firm = dbContext.Firms.Where(s => s.id == firmId).First();
            return View(firm);
        }
        public string GetLocationBuses(int? firmID)
        {
            string query = "Select s1.* from RoutePoints as s1 inner join(select max(createDate) as result from RoutePoints where createDate > Convert(date, Getdate()) and firmId = " + firmID + " group by busId) as s2 on s1.createDate = s2.result";
            List<RoutePoint> maxroutePoints = dbContext.RoutePoints.SqlQuery(query).ToList();
            string jsonString = JsonConvert.SerializeObject(maxroutePoints);
            return jsonString;
        }
    }
}