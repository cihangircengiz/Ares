using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ares.Helpers
{
    public class LoginInfo
    {
        aresdbEntities dbContext = new aresdbEntities();

        public string GetUserId()
        {
            if (HttpContext.Current.Request.Cookies["userId"] != null)
            {
                return HttpContext.Current.Request.Cookies["userId"].Value;
            }
            else
                return string.Empty;
        }

        public string GetFirmId()
        {
            //string cookie = HttpContext.Request.Cookies["firmId"].Value;
            //return cookie;
            if (HttpContext.Current.Request.Cookies["firmId"] != null)
            {
                return HttpContext.Current.Request.Cookies["firmId"].Value;
            }
            else
                return string.Empty;
        }

        public string GetFirmLoginName()
        {       
            string id = GetUserId();
            if(!String.IsNullOrEmpty(id))
            {
                int cInt = Convert.ToInt32(id);
                FirmLogin firmLogin = dbContext.FirmLogins.FirstOrDefault(f => f.id == cInt);
                return firmLogin.name;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}