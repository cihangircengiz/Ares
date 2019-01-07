using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.Helpers;
using Ares.Models;
namespace Ares.Controllers
{
    public class RouteCalculateController : Controller
    {
        // GET: RouteCalculate
        aresdbEntities dbContext = new aresdbEntities();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RouteOptimize(int schoolID,int orderType,int seatCount)
        {
            int defaultSchoolId = schoolID;
            List<Student> studentList = new List<Student>();
            studentList = dbContext.Students.Where(s => s.schoolId == defaultSchoolId).ToList();
            School school = dbContext.Schools.Where(s => s.id == defaultSchoolId).FirstOrDefault();
            foreach (var item in studentList)
            {
                item.Station.routeId = null;
            }
            dbContext.Routes.RemoveRange(dbContext.Routes.Where(s => s.schoolId == defaultSchoolId));
            dbContext.SaveChanges();
            Location destination = new Location();
            destination.lng = school.lng;
            destination.lat = school.lat;

            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());

            Distance _schoolToStudent = MapHelper.farthestLocation(studentList, destination, false);
            studentList.Remove(_schoolToStudent.student);
            int koltukSay = seatCount;
            double routeSayi = studentList.Count / koltukSay;
            double kalan = studentList.Count % koltukSay;
            if (kalan != 0)
                routeSayi++;
            for(int a = 1; a <= routeSayi; a++) { 
            {
                    List<Student> route1 = new List<Student>();
                    Geocode geocode = new Geocode();
                    Route route = new Route();
                    //string _formatedAdress = _schoolToStudent.student.address.Replace("/", " ");
                    //geocode = MapHelper.getGeocode(_formatedAdress);
                    route.name = a.ToString();
                    route.averageTime = 0;
                    route.startAddress = _schoolToStudent.student.address;
                    // route.startAddress = geocode.results[0].formatted_address;
                    route.startLat = _schoolToStudent.student.Station.Lat;
                    //route.startLng = geocode.results[0].geometry.location.lng;
                    //geocode = MapHelper.getGeocode(school.address);
                    route.finishAddress = _schoolToStudent.student.School.address;
                    route.finishLat = _schoolToStudent.student.School.lat;
                    route.finishLng = _schoolToStudent.student.School.lng;
                    route.schoolId = defaultSchoolId;
                    route.createdDate = DateTime.Now;
                    route.isOnline = false;
                    route.firmId = firmId;
                    route.description = "Otomatik Oluşturulan Route";
                    dbContext.Routes.Add(route);
                    _schoolToStudent.student.Station.routeId = route.id;
                    route1.Add(_schoolToStudent.student);
                    List<Distance> yakinlar = MapHelper.closerLocation(studentList, _schoolToStudent.location);
                    try
                    {
                        for (int i = 1; i <= koltukSay; i++)
                        {
                            if(orderType == 1) { 
                                Student _tmp = new Student();
                                _tmp = yakinlar[i - 1].student;
                                _tmp.Station.routeId = route.id;
                                route1.Add(_tmp);
                                studentList.Remove(_tmp);
                            }
                            else
                            {
                                Student _tmp = new Student();
                                _schoolToStudent = MapHelper.farthestLocation(studentList, _schoolToStudent.location, true);
                                if (_schoolToStudent != null)
                                {
                                    _tmp = _schoolToStudent.student;
                                    _tmp.Station.routeId = route.id;
                                    route1.Add(_tmp);
                                    studentList.Remove(_tmp);
                                }
                        }
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.ToString());
                    }
                    foreach (var std in route1)
                    {
                        Student student = dbContext.Students.Where(s => s.id == std.id).First();
                        student.Station.routeId = route.id;
                    }
                    if(orderType == 1)
                        _schoolToStudent = MapHelper.farthestLocation(studentList, destination, false);
                    else
                    {
                        _schoolToStudent = MapHelper.farthestLocation(studentList, destination, true);
                    }
                    dbContext.SaveChanges();
                }
            }

            //Location location = new Location();
            //location.lat = _schoolToStudent.student.Station.Lat;
            //location.lng = _schoolToStudent.student.Station.Lng;
            //Distance _studentToStudent = MapHelper.farthestLocation(studentList, destination, true);

            return Redirect("~/Route/SchoolRoute?schoolId="+defaultSchoolId);
            // return View(studentList);
        }

    }
}