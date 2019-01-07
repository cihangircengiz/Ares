using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ares.EntityData.Model;

namespace Ares.Models
{
    public class UserInformation
    {
        public SysUser sysUser { get; set; }
        public Parent parent { get; set; }
        public List<Notification> notificationList { get; set; }
        public List<Student> studentList { get; set; }
        public Bus bus { get; set; }
        public int unReadCount { get; set; }
        public List<Route> routeList { get; set; }
        public List<Station> stationList { get; set; }
        public Route route { get; set; }
    }
}