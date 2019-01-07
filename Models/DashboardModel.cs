using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ares.EntityData.Model;
namespace Ares.Models
{
    public class DashboardModel
    {
        public int studentCount { get; set; }
        public int parentCount { get; set; }
        public int busCount { get; set; }
        public int stationCount { get; set; }
        public  int studentList { get; set; }
        public int schoolCount { get; set; }
        public List<Student> lastFiveStudent { get; set; }
        public List<Bus>lastFiveBus { get; set; }
        public List<Route> lastFiveRoute { get; set; }
        public List<School> lastFiveSchool { get; set; }
        public int routeCount { get; set; }
    }
}