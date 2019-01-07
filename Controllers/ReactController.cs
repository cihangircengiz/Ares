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
using System;
using RestSharp;

namespace Ares.Controllers
{
    public class ReactController : Controller
    {
        aresdbEntities dbContext = new aresdbEntities();
        // GET: React
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string DoLogin(string userName ,string passwrd)
        {
            SysUser sysUser = dbContext.SysUsers.FirstOrDefault(s => s.email == userName || s.username == userName && s.password == passwrd);
            if (sysUser == null || sysUser.id == 0) {
                return null;
            }
            if (sysUser.type == AppSettingHelper.GetParentLoginSettingId())
            {
                Parent parent = dbContext.Parents.FirstOrDefault(s => s.id == sysUser.typeId);
                UserInformation result = new UserInformation();
                result.sysUser = sysUser;
                result.parent = parent;
                var notification = dbContext.Notifications.Where(n => n.sysUserId == sysUser.id && n.firmId == sysUser.firmId).OrderByDescending(c => c.createdDate).ToList();
                result.unReadCount = notification.Where(n => n.isRead == false).Count();
                return JsonConvert.SerializeObject(result);
            }
            else if(sysUser.type == AppSettingHelper.GetBusLoginSettingId())
            {
                return JsonConvert.SerializeObject(sysUser);
            }
            return null;
        }
        [HttpPost]
        public string GetstudentList(int parentID)
        {
            UserInformation students = new UserInformation();
            students.studentList = dbContext.Students.Where(s => s.parentId == parentID).ToList();
            return JsonConvert.SerializeObject(students);
        }
        [HttpPost]
        public string GetnotificationList(int parentID)
        {
            UserInformation notification = new UserInformation();
            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            notification.notificationList = dbContext.Notifications.Where(n => n.sysUserId == parentID && n.createdDate >= dateTime).OrderByDescending(c => c.createdDate).ToList();
            return JsonConvert.SerializeObject(notification);
        }
        [HttpPost]
        public string GetStationList(int parentID)
        {
            UserInformation _temp = new UserInformation();
            _temp.studentList = dbContext.Students.Where(s => s.parentId == parentID).ToList();
            List<Station> stationList = new List<Station>();
            foreach (var item in _temp.studentList)
            {
                stationList.Add(item.Station);
            }
            _temp.stationList = stationList;
            return JsonConvert.SerializeObject(_temp);
        }
        [HttpPost]
        public string GetBus(int busTypeID,int firmID)
        {
            Bus _tmp = new Bus();
            _tmp = dbContext.Buses.Where(s => s.id == busTypeID && s.firmId == firmID).FirstOrDefault();
            return JsonConvert.SerializeObject(_tmp);
        }
        [HttpPost]
        public string GetrouteList(int busTypeID,int firmID)
        {
            UserInformation result = new UserInformation();
            result.routeList = dbContext.Routes.Where(r => r.firmId == firmID && (r.busId == busTypeID || r.busId2 == busTypeID)).ToList();
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="time"></param>
        ///         0 sabah 
        ///         1 akşam
        /// <returns></returns>
        /// 
        [HttpPost]
        public string GetStationByRouteTime(int routeId,int time)
        {
            Route route = dbContext.Routes.FirstOrDefault(r => r.id == routeId);
            var stationListSP = dbContext.Ares_SabahAksamGuzergah(time, routeId).ToList();
            List<int> stationIndexs = new List<int>();
            foreach (var item in stationListSP)
            {
                stationIndexs.Add(item.id);
            }
            List<Student> studentList = dbContext.Students.Where(s => stationIndexs.Contains(s.stationId)).ToList();
            List<Station> stationList = dbContext.Stations.Where(s => stationIndexs.Contains(s.id)).ToList();
            //foreach (var item in stationList)
            //{
            //    foreach (var item2 in item.Students)
            //    {
            //        studentList.Add(item2);
            //    }
            //}
            UserInformation userInformation = new UserInformation();
            userInformation.route = route;
            if (time == 0) { 
                userInformation.stationList = stationList.OrderBy(s => s.orderNumber).ToList();
                userInformation.studentList = studentList.OrderBy(s => s.Station.orderNumber).ToList();
            }
            else
            {

                userInformation.stationList = stationList.OrderByDescending(s => s.orderNumber).ToList();
                userInformation.studentList = studentList.OrderByDescending(s => s.Station.orderNumber).ToList();
            }
                //userInformation.stationList = stationList.OrderByDescending(s => s.orderNumber).ToList();
            
            string jsonString = JsonConvert.SerializeObject(userInformation);
            return jsonString;
        }

        /// <summary>
        /// /// old parameter sadece sabah geliyor
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="firmID"></param>
        /// <param name="busID"></param>
        /// <param name="routeID"></param>
        /// <returns></returns>
        [HttpPost]
        public string SetServiceLocation(double lat, double lng, int firmID, int busID, int routeID)
        {
            GetDistanceInformation(lng.ToString(), lat.ToString(), routeID,0);
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
        /// <summary>
        /// new by time
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="firmID"></param>
        /// <param name="busID"></param>
        /// <param name="routeID"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public string SetServiceLocationbyTime(double lat, double lng, int firmID, int busID, int routeID,int time)
        {
            GetDistanceInformation(lng.ToString(), lat.ToString(), routeID,time);
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
        public void GetDistanceInformation(string mServiceLng, string mServiceLat,int routeID,int time)
        {
            var cutoff = DateTime.Now.AddMinutes(-3); /// 3 dakika içinde atılmış bildirimleri kontrol ediyor
            List<Station> stations = null;
            if (time == 0)
                 stations = dbContext.Stations.Where(s => s.routeId == routeID).OrderBy(s=> s.orderNumber).ToList();
            else
            {
                stations = dbContext.Stations.Where(s => s.routeId == routeID).OrderByDescending(s => s.orderNumber).ToList();
            }
            var delayedTime = DateTime.Now.AddHours(-1); //// 1 saatlik dilim içinde atılmış bildirimleri kontrol ediyor
            List<SysUser> sysUsers = dbContext.SysUsers.Join(dbContext.Notifications,
                s => s.id,
                sa => sa.sysUserId,
                (s, sa) => new { sysuser = s, notification = sa }).Where(sa => sa.notification.createdDate >= delayedTime && sa.sysuser.type == 8 && sa.notification.type == "2").
                Select(sa => sa.sysuser).ToList(); ;
            // List<Notification> notifications = dbContext.Notifications.Where(s=> s.createdDate >= cutoff).ToList();
            var tmpStations = stations.ToList();
            foreach (var item in tmpStations)
            {
                foreach (var parent in sysUsers)
                {
                    if(item.Students.First().Parent.id == parent.typeId && parent.type == 8)
                    {
                        stations.Remove(item);
                    }
                }
               
            }
            List<Notification> notifications = null;
            SysUser sysUser = null;
            RestClient client = null;
            MobileController mobileController = new MobileController();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            var request = new RestRequest(Method.GET);
            //request.AddHeader("Postman-Token", "32133d55-cfee-4c5d-a1de-e6418a09c95a");
            request.AddHeader("Cache-Control", "no-cache");
            List<Distance> jsonList = new List<Distance>();
            stations = stations.Take(3).ToList();
            foreach (var item in stations)
            {
                client = new RestClient("http://185.115.242.209:5000/route/v1/driving/" + mServiceLng.Replace(",", ".") + "," + mServiceLat.Replace(",", ".") + ";" + item.Lng.ToString().Replace(",", ".") + "," + item.Lat.ToString().Replace(",", ".") + "?overview=false");
                string result = "http://185.115.242.209:5000/route/v1/driving/" + mServiceLng.Replace(",", ".") + "," + mServiceLat.Replace(",", ".") + ";" + item.Lng.ToString().Replace(",", ".") + "," + item.Lat.ToString().Replace(",", ".") + "?overview=false";
                Console.WriteLine(result);
                IRestResponse response = client.Execute(request);
                jsonList.Add(JsonConvert.DeserializeObject<Distance>(response.Content,settings));
            }
            for (int i = 0; i < stations.Count; i++)
            {
                if (stations[i].range == 0)
                    stations[i].range = 1500; /// eğer 0 gelirse 1500 standart tanımlama yapıldı.
                if(jsonList[i].routes[0].distance <= stations[i].range)
                {
                    ///// Send Notification to Parent
                    int RelatedStudentsParentID = stations[i].Students.First().parentId;
                    sysUser = dbContext.SysUsers.FirstOrDefault(s => s.typeId == RelatedStudentsParentID);
                    notifications = dbContext.Notifications.Where(s => s.createdDate >= cutoff && s.sysUserId == sysUser.id && s.type == "2").ToList();
                    if(notifications.Count <= 0)
                        mobileController.NotificationInformation("2",sysUser.id.ToString(),sysUser.firmId,stations[i].Students.First().id,routeID,"0");

                }
            }
            //string url = "http://185.115.242.209:5000/route/v1/driving/" + mServiceLng + "," + mServiceLat + ";" + mStudentLng + "," + mStudentLat + "?overview=false";
            // return null;
        }
    }
}