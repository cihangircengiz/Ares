using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ares.EntityData.Model;

namespace Ares.Models
{
    public class RouteModel
    {
        public Route route { get; set; }
        public List<Station> stationList { get; set; }
        public List<School> schoolList { get; set; }
        public List<Bus> busList { get; set; }
        public Bus selectedBus { get; set; }
        public Bus secondBus { get; set; }
        public School selectedSchool { get; set; }

        public List<Student> studentList { get; set; }
    }

    public class ChangeStationModel
    {
        public string location { get; set; }
        public int stationId { get; set; }
        public int order { get; set; }
    }
}