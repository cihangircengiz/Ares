using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ares.Models;
using Ares.Helpers;
using System.Data;
using System.IO;
using OfficeOpenXml;
namespace Ares.Controllers
{

    public class SchoolController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
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
        public ActionResult Create()
        {
            return View();
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Create(School obj)
        {
            LoginInfo loginInfo = new LoginInfo();
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            obj.firmId = Convert.ToInt32(loginInfo.GetFirmId());
            Geocode geocode = new Geocode();
            geocode = MapHelper.getGeocode(obj.address,obj.city);
            obj.lat = geocode.results[0].geometry.location.lat;
            obj.lng = geocode.results[0].geometry.location.lng;
            obj.address = geocode.results[0].formatted_address;
            obj.createdDate = DateTime.Now;
            dbContext.Schools.Add(obj);
            dbContext.SaveChanges();
            return Redirect("/School/Index");
        }
        [_SessionControl]
        public ActionResult Detail(int? id)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = Convert.ToInt32(loginInfo.GetFirmId());
            if (id == 0 || id == null)
                return Redirect("/School/Index");
            School school = new School();
            school = dbContext.Schools.FirstOrDefault(s => s.id == id && s.firmId == firmId);
            return View(school);
        }
        [_SessionControl]
        [HttpPost]
        public ActionResult Detail(School obj)
        {
            School school = new School();
            school = dbContext.Schools.FirstOrDefault(s => s.id == obj.id);
            List<Student> students = dbContext.Students.Where(s => s.schoolId == school.id).ToList();
            List<int> vs = new List<int>();
            if (Request.Form["Delete"] != null)
            {
                /// delete routes 
                foreach (var item in school.Routes.ToList())
                {
                    List<Station> stations = dbContext.Stations.Where(s => s.routeId == item.id).ToList();
                    foreach (var stad in stations)
                    {
                        stad.routeId = null;
                    }
                    dbContext.Routes.Remove(item);
                }
                /// delete all related station
                foreach (var item in students)
                {
                    dbContext.Stations.RemoveRange(dbContext.Stations.Where(s => s.id == item.stationId));
                    //item.Station.routeId = null;
                }
                /// delete all student
                dbContext.Students.RemoveRange(dbContext.Students.Where(s => s.schoolId == obj.id));
                dbContext.Schools.Remove(school);
                dbContext.SaveChanges();

            }
            else
            {
                school.name = obj.name;
                school.email = obj.email;

                Geocode geocode = new Geocode();
                geocode = MapHelper.getGeocode(obj.address,obj.city);
                if (obj.address != school.address)
                {
                    obj.address = geocode.results[0].formatted_address;
                    obj.lat = geocode.results[0].geometry.location.lat;
                    obj.lng = geocode.results[0].geometry.location.lng;
                }

                school.address = obj.address;
                school.phone = obj.phone;
                school.city = obj.city;
                school.town = obj.town;
                school.authorizePersonName = obj.authorizePersonName;
                school.authorizePersonMail = obj.authorizePersonMail;
                school.authorizePersonPhone = obj.authorizePersonPhone;
                school.lat = obj.lat;
                school.lng = obj.lng;
                dbContext.SaveChanges();
                return Redirect("/School/Index");
            }
            return Redirect("/School/Index");
        }
        public ActionResult Import()
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
        [HttpPost]
        public ActionResult Import(FormCollection formCollection, int? id)
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = Convert.ToInt32(loginInfo.GetFirmId());
            List<String> ErrorList = new List<String>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["file"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))   
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    var usersList = new List<ImportDataModel>();
                    /// Read All Rows from XLS
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        // var noOfCol = 8;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)   
                        {
                            var user = new ImportDataModel();
                            try
                            {
                                user.ogrAd = workSheet.Cells[rowIterator, 1].Value.ToString();
                                user.ogrTcNo = workSheet.Cells[rowIterator, 2].Value.ToString();
                                user.adres = workSheet.Cells[rowIterator, 3].Value.ToString();
                                user.fiyat = workSheet.Cells[rowIterator, 4].Value.ToString();
                                user.veliAdi = workSheet.Cells[rowIterator, 5].Value.ToString();
                                user.veliEposta = workSheet.Cells[rowIterator, 6].Value.ToString();
                                user.veliTelefon = workSheet.Cells[rowIterator, 7].Value.ToString();
                                user.veliTcNo = workSheet.Cells[rowIterator, 8].Value.ToString();
                            }
                            catch (NullReferenceException)
                            {
                                // Console.WriteLine("Taştı" + er.ToString());
                                user = null;
                            }
                            if(user != null)
                                usersList.Add(user);
                        }
                        //// Create Parent And Student Action
                        foreach (var item in usersList)
                        {
                            Parent _tmp = dbContext.Parents.Where(s => s.email == item.veliEposta).FirstOrDefault();
                            SysUser _sysuser = dbContext.SysUsers.Where(s => s.email == item.veliEposta).FirstOrDefault();
                            
                            if(_tmp == null && _sysuser == null)
                            {
                                // Create Parent And Student
                                _tmp = new Parent();
                                _tmp.name = item.veliAdi;
                                _tmp.phone = item.veliTelefon;
                                _tmp.tcNo = item.veliTcNo;
                                _tmp.firmId = firmId;
                                _tmp.createdDate = DateTime.Now;
                                _tmp.address = item.adres;
                                _tmp.email = item.veliEposta;
                                dbContext.Parents.Add(_tmp);
                                dbContext.SaveChanges();
                                //// 
                                SysUser sysUser = new SysUser();
                                sysUser.email = _tmp.email;
                                sysUser.createdDate = DateTime.Now;
                                sysUser.firmId = firmId;
                                sysUser.name = _tmp.name;
                                sysUser.type = AppSettingHelper.GetParentLoginSettingId();
                                sysUser.typeId = _tmp.id;
                                sysUser.username = _tmp.phone;
                                sysUser.password = item.ogrTcNo;
                                dbContext.SysUsers.Add(sysUser);
                                dbContext.SaveChanges();
                            }
                            // int firmId = Convert.ToInt32(loginInfo.GetFirmId());
                            // Add Student
                                School school = dbContext.Schools.Where(s => s.id == id).FirstOrDefault();
                                Geocode geocode = new Geocode();
                                geocode = MapHelper.getGeocode(item.adres,school.city);
                                Station station = new Station();
                                try
                                {
                                    station.Lat = geocode.results[0].geometry.location.lat;
                                    station.Lng = geocode.results[0].geometry.location.lng;
                                    station.stationAddress = geocode.results[0].formatted_address;
                                }
                                catch (Exception)
                                {
                                    ErrorList.Add(item.ogrAd + "  Adresi Hatalı");
                                    
                                    station.Lat = school.lat;
                                    station.Lng = school.lng;
                                    station.stationAddress = item.adres;

                            }    
                                // station.stationName = item.stationName;
                                //station.routeId = 1;
                                station.createdDate = DateTime.Now;
                                station.firmId = firmId;
                                station.range = 1500;
                                dbContext.Stations.Add(station);
                                dbContext.SaveChanges();

                                Student student = new Student();
                                student.name = item.ogrAd;
                                student.tcNo = item.ogrTcNo;
                                student.address = station.stationAddress;
                                student.phone = item.veliTelefon;
                                student.bloodGroup = "";
                                student.firmId = firmId;
                                student.stationId = station.id;
                                student.schoolId = id ?? 0;
                                student.createdDate = DateTime.Now;
                                student.serviceProperty = 6;
                                student.totalAmount = Convert.ToDouble(item.fiyat);
                                student.parentId = _tmp.id;
                                dbContext.Students.Add(student);
                                dbContext.SaveChanges();
                        }


                    }
                }
            }
            ViewBag.Errors = ErrorList;
            //return View();
            return Redirect("/School/Detail?id="+id);
        }
    }
}