using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ares.EntityData.Model;

namespace Ares.Models
{
    public class ParentCrudModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string tcNo { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public DateTime createdDate { get; set; }
        public int firmId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public int sysUserId { get; set; }
        public List<Student> studentList { get; set; }

    }
}