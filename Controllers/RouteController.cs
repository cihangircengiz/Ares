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
    public class RouteController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: Route
        [_SessionControl]
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            List<School> schoolList = new List<School>();
            schoolList = dbContext.Schools.Where(s => s.firmId == firmId).ToList();
            return View(schoolList);
        }
        [_SessionControl]
        public ActionResult SchoolRoute(int? schoolId)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            if (schoolId == 0 || schoolId == null)
                return Redirect("/Route/Index");

            SchoolRouteModel model = new SchoolRouteModel();
            List<Student> studentList = dbContext.Students.Where(s => s.firmId == firmId && s.schoolId == schoolId).ToList();
            model.studentList = studentList;
            model.routeList = dbContext.Routes.Where(r => r.schoolId == schoolId && r.firmId == firmId).ToList();
            //if (model.studentList.Count != 0 && model.routeList.Count != 0)
            return View(model);
        }
        [_SessionControl]
        public ActionResult Create()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            RouteCrudModel model = new RouteCrudModel();
            model.busList = dbContext.Buses.Where(b => b.firmId == firmId).ToList();
            model.schoolList = dbContext.Schools.Where(s => s.firmId == firmId).ToList();
            return View(model);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Create(Route obj)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());

            Geocode geocode = new Geocode();
            obj.School = dbContext.Schools.Where(s => s.id == obj.schoolId).First();
            geocode = MapHelper.getGeocode(obj.startAddress,obj.School.city);
            Route route = new Route();
            route.name = obj.name;
            route.averageTime = obj.averageTime;
            route.startAddress = geocode.results[0].formatted_address;
            route.startLat = geocode.results[0].geometry.location.lat;
            route.startLng = geocode.results[0].geometry.location.lng;
            geocode = MapHelper.getGeocode(obj.finishAddress,obj.School.city);
            route.finishAddress = geocode.results[0].formatted_address;
            route.finishLat = geocode.results[0].geometry.location.lat;
            route.finishLng = geocode.results[0].geometry.location.lng;
            route.description = obj.description;
            route.busId = obj.busId;
            route.busId2 = obj.busId2;
            route.schoolId = obj.schoolId;
            route.createdDate = DateTime.Now;
            route.isOnline = false;
            route.firmId = firmId;
            dbContext.Routes.Add(route);
            dbContext.SaveChanges();

            return Redirect("/Route/SchoolRoute?schoolId=" + route.schoolId);
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
                return Redirect("/Route/Index");

            Route route = dbContext.Routes.FirstOrDefault(r => r.id == id && r.firmId == firmId);
            if (route == null)
                return Redirect("/Route/Index");
            List<Station> stationList = dbContext.Stations.Where(s => s.routeId == route.id && s.firmId == firmId).ToList();
            RouteModel model = new RouteModel();
            model.secondBus = route.Bus1;
            model.route = route;
            model.stationList = stationList;
            model.busList = dbContext.Buses.Where(b => b.firmId == firmId && b.id!=route.busId).ToList();
            model.schoolList = dbContext.Schools.Where(s => s.firmId == firmId && s.id==route.schoolId).ToList();
            model.selectedSchool = route.School;
            model.selectedBus = route.Bus;
            model.secondBus = route.Bus1;
            return View(model);
        }
        [HttpPost]
        public ActionResult Detail(Route obj)
        {
            Route route = dbContext.Routes.FirstOrDefault(r => r.id == obj.id);
            if (Request.Form["Delete"] != null)
            {
                List<Station> stations = dbContext.Stations.Where(s => s.routeId == route.id).ToList();
                foreach (var item in stations)
                {
                    item.routeId = null;
                }
                dbContext.Routes.Remove(route);
                dbContext.SaveChanges();
            }
            else { 
                route.averageTime = obj.averageTime;
                route.name = obj.name;
                route.description = obj.description;
                Geocode geocode = new Geocode();
                geocode = MapHelper.getGeocode(obj.startAddress, route.School.city);
                route.startAddress = geocode.results[0].formatted_address;
                route.startLat = geocode.results[0].geometry.location.lat;
                route.startLng = geocode.results[0].geometry.location.lng;
                geocode = MapHelper.getGeocode(obj.finishAddress, route.School.city);
                route.finishAddress = geocode.results[0].formatted_address;
                route.finishLat = geocode.results[0].geometry.location.lat;
                route.finishLng = geocode.results[0].geometry.location.lng;
                route.busId = obj.busId;
                route.busId2 = obj.busId2;
                dbContext.SaveChanges();
                }
            return Redirect("/Route/Detail?id=" + route.id);
        }

        [HttpPost]
        public ActionResult ChangeStationOrder(List<ChangeStationModel> objList)
        {
            foreach (var item in objList)
            {
                Station station = dbContext.Stations.FirstOrDefault(s => s.id == item.stationId);
                station.orderNumber = item.order;
                dbContext.SaveChanges();
            }
            return null;
        }
        [HttpPost]
        public ActionResult SingleOrderChange(int? stationID,int? order)
        {
            Station station = dbContext.Stations.FirstOrDefault(s => s.id == stationID);
            station.orderNumber = order;
            dbContext.SaveChanges();
            return null;
        }
    }
}