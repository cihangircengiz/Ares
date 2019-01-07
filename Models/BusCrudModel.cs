using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ares.EntityData.Model;
namespace Ares.Models
{
    public class BusCrudModel
    {
        public int id { get; set; }
        public string plate { get; set; }
        public string model { get; set; }
        public string brand { get; set; }
        public string authorizePersonName { get; set; }
        public string authorizePersonMail { get; set; }
        public string authorizePersonPhone { get; set; }
        public int firmId { get; set; }
        public DateTime createdDate { get; set; }
        public int maxSeatCount { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int sysUserId { get; set; }
        public List<Route> busRoutes { get; set; }
        public List<Student> studentList { get; set; }
    }
}