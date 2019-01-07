using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ares.EntityData.Model;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Ares.Helpers;
using Ares.Models;

namespace Ares.Controllers
{
    public class MobileController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ExampleJsonFunction()
        {
            return Json(new { name = "john", age = 31, city = "New York" }, JsonRequestBehavior.AllowGet);
        }

        public string GetFirm(int id)
        {
            Firm firm = new Firm();
            firm = dbContext.Firms.FirstOrDefault(s => s.id == id);
            string firmString = JsonConvert.SerializeObject(firm);

            //Eğer herhangi bir collection kullanılması gerekirse aşağıdaki firmString kodu kullanılmalıdır.

            //string firmString = JsonConvert.SerializeObject(firm, Formatting.None,
            //            new JsonSerializerSettings()
            //            {
            //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //            });

            return firmString;
        }
        public string GetFirmList()
        {
            List<Firm> firmList = new List<Firm>();
            firmList = dbContext.Firms.ToList();
            string jsonString = JsonConvert.SerializeObject(firmList);
            return jsonString;
        }
        public void BreakPointTest()
        {
            string obj = GetFirm(1);
            Firm firm = JsonConvert.DeserializeObject<Firm>(obj);

            string obj2 = GetFirmList();
            List<Firm> firmList = JsonConvert.DeserializeObject<List<Firm>>(obj2);

            //Bu fonksiyonun başına breakpoint koyarak nesnelerin veya listlerin dolup dolmadığını kontrol edilebilir.
        }
        [HttpPost]
        public string GetUserInfo(string email, string password)
        {
            SysUser user = new SysUser();
            user = dbContext.SysUsers.Where(s => s.email == email && s.password == password).FirstOrDefault();
            UserInformation userInformation = new UserInformation();
            userInformation.sysUser = user;
            if (user == null || user.id == 0)
            {
                return null;
            }
            if (user.type == AppSettingHelper.GetParentLoginSettingId())
            {
                userInformation.parent = dbContext.Parents.FirstOrDefault(p => p.id == user.typeId && p.firmId == user.firmId);
                userInformation.studentList = dbContext.Students.Where(s => s.parentId == userInformation.parent.id).ToList();
                userInformation.notificationList = dbContext.Notifications.Where(n => n.sysUserId == user.id && n.firmId == user.firmId).OrderByDescending(c=> c.createdDate).ToList();
                userInformation.unReadCount = userInformation.notificationList.Where(n => n.isRead == false).Count();
                List<Station> stationList = new List<Station>();
                foreach (var item in userInformation.studentList)
                {
                    stationList.Add(item.Station);
                }
                userInformation.stationList = stationList;
            }
            else if (user.type == AppSettingHelper.GetBusLoginSettingId())
            {
                userInformation.bus = dbContext.Buses.FirstOrDefault(p => p.id == user.typeId && p.firmId == user.firmId);
                userInformation.routeList = dbContext.Routes.Where(r => r.firmId == user.firmId && r.busId == userInformation.bus.id).ToList();
            }
            else
            {
                return "";
            }

            string jsonString = JsonConvert.SerializeObject(userInformation);
            return jsonString;
        }
        [HttpPost]
        public string GetStationByRoute(int routeId)
        {
            Route route = dbContext.Routes.FirstOrDefault(r => r.id == routeId);
            List<Station> stationList = dbContext.Stations.Where(s=> s.routeId==route.id).OrderBy(s => s.orderNumber).ToList();
            List<Student> studentList = new List<Student>();
            foreach (var item in stationList)
            {
                foreach (var item2 in item.Students)
                {
                    studentList.Add(item2);
                }
            }
            UserInformation userInformation = new UserInformation();
            userInformation.route = route;
            userInformation.stationList = stationList;
            userInformation.studentList = studentList;
            string jsonString = JsonConvert.SerializeObject(userInformation);
            return jsonString;
        }
        [HttpPost]
        public bool UpdateRange(int parentId,int range)
        {
            Parent parent = dbContext.Parents.FirstOrDefault(r => r.id == parentId);
            foreach (var item in parent.Students)
            {
                item.Station.range = range;
            }
            dbContext.SaveChanges();
            return true;
        }
        [HttpPost]
        public string GetRangeInformation(int parentID)
        {
            Parent parent = dbContext.Parents.FirstOrDefault(r => r.id == parentID);
            string jsonString = JsonConvert.SerializeObject(parent.Students.First().Station);
            return jsonString;
        }
        [HttpPost]
        public string UpdateEmailandPassword(string email, string password, int id)
        {
            try
            {
                SysUser parent = dbContext.SysUsers.FirstOrDefault(s => s.id == id && s.email == email);
                parent.password = password;
                dbContext.SaveChanges();
                return "Success";
            }
            catch (Exception)
            {
                return null;
            }
            
        }
        [HttpPost]
        public string SetServiceLocation(double lat, double lng, int firmID,int busID,int routeID)
        {
            RoutePoint _tmp = new RoutePoint();
            _tmp.Lat = lat;
            _tmp.Lng = lng;
            _tmp.firmId = firmID;
            _tmp.busId = busID;
            _tmp.routeId = routeID;
            _tmp.createDate = DateTime.Now;
            dbContext.RoutePoints.Add(_tmp);
            dbContext.SaveChanges();
            return null;
        }
        public string MyFunction()
        {
            List<Student> studentList = dbContext.Students.Where(s => s.schoolId == 1004).ToList();
            School school = dbContext.Schools.FirstOrDefault(s => s.id == 1004);
            return JsonConvert.SerializeObject(school.address);
        }
        [HttpPost]
        public void NotificationInformation(string type,string sysUserID,int firmID,int stID,int routeID,string status)
        {
            Student student = new Student();
            try
            {
                student = dbContext.Students.FirstOrDefault(s => s.id == stID);
            }
            catch (Exception)
            {

                throw;
            }
            List<Student> studentlist = dbContext.Students.Where(s => s.Station.routeId == routeID).ToList();
            string _message;
            switch (type)
            {
                case "1":////////// bus on way
                    _message = "Servis aracı yola çıkmıştır.";
                    foreach (var item in studentlist)
                    {
                        var tmp = ReturnSysUserId(item).ToString();
                        SendOnesignalMessageSingle(tmp, _message);
                        dbContext.Notifications.Add(logMessage(ReturnSysUserId(item),type,firmID,_message));
                        dbContext.SaveChanges();
                    }
                    break;
                case "4":
                    _message = "Servis aracı güzergahı tamamladı.";
                    foreach(var item in studentlist)
                    {
                        SendOnesignalMessageSingle(ReturnSysUserId(item).ToString(), _message);
                        dbContext.Notifications.Add(logMessage(ReturnSysUserId(item), type, firmID, _message));
                        dbContext.SaveChanges();
                    }
                    break;
                case "2":
                    _message = "Servis aracı bölgenize giriş yapmıştır";
                    SendOnesignalMessageSingle(ReturnSysUserId(student).ToString(), _message);
                    dbContext.Notifications.Add(logMessage(Convert.ToInt32(ReturnSysUserId(student)), type, firmID, _message));
                    dbContext.SaveChanges();
                    //student.id
                    break;
                case "3":
                    _message = student.name+" adlı öğrencimiz servise "+status;
                    SendOnesignalMessageSingle(ReturnSysUserId(student).ToString(), _message);
                    dbContext.Notifications.Add(logMessage(Convert.ToInt32(ReturnSysUserId(student)), type, firmID, _message));
                    dbContext.SaveChanges();
                    break;
            }

        }

        private int ReturnSysUserId(Student input)
        {
            return dbContext.SysUsers.FirstOrDefault(s => s.typeId == input.Parent.id).id;
        }
        private Notification logMessage(int sysUserID,string type,int firmID,string _message)
        {
            Notification notification = new Notification();
            notification.isRead = false;
            notification.sysUserId = sysUserID;
            notification.createdDate = DateTime.Now;
            notification.type = type;
            notification.firmId = firmID;
            notification.contentText = _message;
            return notification;
        }
        public void SendOnesignalMessageSingle(string sysuserID,string _msg)
        {
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic MDdiMDQzZWEtMTdhOS00NDAyLTkxNzktNjEzNGY4NjJiMGM3");

            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = "f20e7902-0783-4c47-a3bb-64e970592346",
                contents = new { en = _msg },
                headings = new { en = "Ares Servis" },
                filters = new object[] { new { field = "tag", key = "userID", value = sysuserID } }
            }; // value userID ile değişecek



            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
        }
        public void NotificationReadAll(int sysUserID)
        {
            List<Notification> notificationList = dbContext.Notifications.Where(s => s.sysUserId == sysUserID && s.isRead == false).ToList();
            foreach(var item in notificationList)
            {
                item.isRead = true;
                dbContext.SaveChanges();
            }
        }
        public string UpdateUserAddress(int id, double lat, double lng)
        {
            try
            {
                Geocode _tmp = MapHelper.getGeocode(lat, lng);
                Station _userStation = dbContext.Stations.FirstOrDefault(s => s.id == id);
                Student _user = dbContext.Students.FirstOrDefault(s => s.stationId == id);
                _userStation.Lng = lng;
                _userStation.Lat = lat;
                _userStation.stationAddress = _tmp.results[0].formatted_address;
                _user.address = _tmp.results[0].formatted_address;
                dbContext.SaveChanges();
                return "Success";
            }
            catch (Exception)
            {
                return null;
            }
            
        }
    }
}