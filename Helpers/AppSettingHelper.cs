using Ares.EntityData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ares.Helpers
{
    public class AppSettingHelper
    {
        
        public static string loginKey = "LoginProcess";
        public static string parentLoginValue = "Parent";
        public static string busLoginValue = "Bus";


        public static int GetParentLoginSettingId()
        {
            aresdbEntities dbContext = new aresdbEntities();
            AppSetting appSetting  = dbContext.AppSettings.FirstOrDefault(s => s.settingKey == loginKey && s.settingValue==parentLoginValue);
            if (appSetting != null || appSetting.id != 0)
                return appSetting.id;
            else
                return 0;
        }
        public static int GetBusLoginSettingId()
        {
            aresdbEntities dbContext = new aresdbEntities();
            AppSetting appSetting = dbContext.AppSettings.FirstOrDefault(s => s.settingKey == loginKey && s.settingValue == busLoginValue);
            if (appSetting != null || appSetting.id != 0)
                return appSetting.id;
            else
                return 0;
        }
    }
}