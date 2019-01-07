using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ares.Models
{
    public class StudentCrudModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tcNo { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string bloodGroup { get; set; }
        public int firmId { get; set; }
        public int stationId { get; set; }
        public int parentId { get; set; }
        public int schoolId { get; set; }
        public DateTime createdDate { get; set; }
        public int serviceProperty { get; set; }
        public double totalAmount { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public int orderNumber { get; set; }
        public int routeId { get; set; }
        public string stationAddress { get; set; }
        public string stationName { get; set; }
        public List<School> schoolList { get; set; }
        public School school { get; set; }
    }
}