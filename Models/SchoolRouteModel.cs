using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ares.Models
{
    public class SchoolRouteModel
    {
        public List<Student> studentList { get; set; }
        public List<Route> routeList { get; set; }
    }
}