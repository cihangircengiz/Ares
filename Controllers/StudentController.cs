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
    public class StudentController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        [_SessionControl]
        // GET: Student
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");   
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            List<Student> studentList = dbContext.Students.Where(s => s.firmId == firmId).ToList();
            return View(studentList);
        }
        [_SessionControl]
        public ActionResult Create(int? pid)
        {
            if (pid == 0 || pid == null)
                return Redirect("/Parent/Index");
            int parentId = Convert.ToInt32(pid);
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            List<School> schoolList = new List<School>();
            schoolList = dbContext.Schools.Where(s => s.firmId == firmId).ToList();
            StudentCrudModel model = new StudentCrudModel();
            model.schoolList = schoolList;
            model.parentId = parentId;
            return View(model);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Create(StudentCrudModel obj)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            obj.school = dbContext.Schools.Where(s => s.id == obj.schoolId).First();
            Geocode geocode = new Geocode();
            geocode = MapHelper.getGeocode(obj.address,obj.school.city);

            Station station = new Station();
            if(obj.lat == 0 && obj.lng == 0) { 
                station.Lat = geocode.results[0].geometry.location.lat;
                station.Lng = geocode.results[0].geometry.location.lng;
            }
            else
            {
                station.Lat = obj.lat;
                station.Lng = obj.lng;

            }
            station.stationAddress = geocode.results[0].formatted_address;      
            station.stationName = obj.stationName;
            //station.routeId = 1;
            station.createdDate = DateTime.Now;
            station.firmId = firmId;
            dbContext.Stations.Add(station);
            dbContext.SaveChanges();

            Student student = new Student();
            student.name = obj.name;
            student.tcNo = obj.tcNo;
            student.address = geocode.results[0].formatted_address;
            student.phone = obj.phone;
            student.bloodGroup = obj.bloodGroup;
            student.firmId = firmId;
            student.Station = station;
            student.stationId = station.id;
            student.schoolId = obj.schoolId;
            student.createdDate = DateTime.Now;
            student.serviceProperty = obj.serviceProperty;
            student.totalAmount = obj.totalAmount;
            student.parentId = obj.parentId;
            dbContext.Students.Add(student);
            dbContext.SaveChanges();

            return Redirect("/Parent/Detail?id=" + student.parentId);
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
            Student student = new Student();
            student = dbContext.Students.FirstOrDefault(s => s.id == id && s.firmId==firmId);
            if (student == null)
                return Redirect("/Parent/Index");
            
            StudentCrudModel model = new StudentCrudModel();
            model.id = student.id;
            model.name = student.name;
            model.address = student.address;
            model.phone = student.phone;
            model.tcNo = student.tcNo;
            model.bloodGroup = student.bloodGroup;
            model.firmId = student.firmId;
            model.stationId = student.stationId;
            model.parentId = student.parentId;
            model.schoolId = student.schoolId;
            model.createdDate = student.createdDate;
            model.serviceProperty = Convert.ToInt32(student.serviceProperty);
            model.totalAmount = Convert.ToDouble(student.totalAmount);

            Station station = dbContext.Stations.FirstOrDefault(s => s.id == student.stationId && s.firmId == firmId);
            model.lat = station.Lat;
            model.lng = station.Lng;
            model.stationAddress = station.stationAddress;
            model.stationName = station.stationName;
            model.schoolList = dbContext.Schools.Where(s => s.firmId == firmId && s.id != student.schoolId).ToList();
            model.school = dbContext.Schools.FirstOrDefault(s => s.id == student.schoolId);

            return View(model);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Detail(StudentCrudModel obj)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());

            Student student = new Student();
            student = dbContext.Students.FirstOrDefault(s => s.id == obj.id);
            obj.school = dbContext.Schools.FirstOrDefault(s => s.id == obj.schoolId);
            if (Request.Form["Delete"] != null)
            {
                dbContext.Students.Remove(student);
                dbContext.SaveChanges();
                return Redirect("/Student/Index");
            }
            else
            {
                Geocode geocode = new Geocode();
                geocode = MapHelper.getGeocode(obj.address,obj.school.city);
                if (obj.address != student.address)
                {
                    obj.address = geocode.results[0].formatted_address;
                    obj.lat = geocode.results[0].geometry.location.lat;
                    obj.lng = geocode.results[0].geometry.location.lng;
                }
                else if(obj.lat != student.Station.Lat && obj.lng != student.Station.Lng)
                {
                    geocode = MapHelper.getGeocode(obj.lat, obj.lng);
                    obj.address = geocode.results[0].formatted_address;
                    obj.lat = geocode.results[0].geometry.location.lat;
                    obj.lng = geocode.results[0].geometry.location.lng;
                }
                student.name = obj.name;
                student.tcNo = obj.tcNo;
                student.address = obj.address;
                student.phone = obj.phone;
                if (obj.bloodGroup != null)
                    student.bloodGroup = obj.bloodGroup;
                else
                    student.bloodGroup = "";
                student.schoolId = obj.schoolId;
                student.serviceProperty = obj.serviceProperty;
                student.totalAmount = obj.totalAmount;
                // dbContext.SaveChanges();

                Station station = new Station();
                station = dbContext.Stations.FirstOrDefault(s => s.id == student.stationId);
                station.Lat = obj.lat;
                station.Lng = obj.lng;
                station.stationAddress = obj.address;
                station.stationName = obj.stationName;
                //station.routeId = 1;
                dbContext.SaveChanges();

                return Redirect("/Parent/Detail?id=" + student.parentId);
            }
        }

        [HttpPost]
        public ActionResult Update(StudentCrudModel obj)
        {
            Student student = new Student();
            student = dbContext.Students.FirstOrDefault(s => s.id == obj.id);
            student.name = obj.name;
            student.phone = obj.phone;
            if(obj.routeId != 0)
                student.Station.routeId = obj.routeId;
            else
            {
                student.Station.routeId = null;
            }
            dbContext.SaveChanges();
            return null;
        }
        [HttpPost]
        public ActionResult LocationUpdateAction(StudentCrudModel obj)
        {
            Student student = new Student();
            student = dbContext.Students.FirstOrDefault(s => s.id == obj.id);
            Geocode geocode = new Geocode();
            geocode = MapHelper.getGeocode(obj.lat, obj.lng);
            student.Station.Lat = obj.lat;
            student.Station.Lng = obj.lng;
            student.address = geocode.results[0].formatted_address;
            student.Station.stationAddress = geocode.results[0].formatted_address;
            dbContext.SaveChanges();
            return null;
        }
    }
}