using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ares.EntityData.Model;
namespace Ares.Models
{
    public class Distance
    {
        public Student student { get; set; }
        public Location location { get
            {
                if(student != null) { 
                    Location _tmp = new Location();
                    _tmp.lat = student.Station.Lat;
                    _tmp.lng = student.Station.Lng;
                    return _tmp;
                }
                else
                {
                    return null;
                }
            }
        }
        public string code { get; set; }
        public List<Route> routes { get; set; }
        public List<Waypoint> waypoints { get; set; }
        public class Leg
        {
            public List<object> steps { get; set; }
            public double distance { get; set; }
            public double duration { get; set; }
            public string summary { get; set; }
            public double weight { get; set; }
        }

        public class Route
        {
            public List<Leg> legs { get; set; }
            public double distance { get; set; }
            public double duration { get; set; }
            public string weight_name { get; set; }
            public double weight { get; set; }
        }

        public class Waypoint
        {
            public string hint { get; set; }
            public string name { get; set; }
            public List<double> location { get; set; }
        }
    }
}