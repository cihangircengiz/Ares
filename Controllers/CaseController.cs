using Ares.EntityData.Model;
using Ares.Helpers;
using Ares.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ares.Controllers
{
    public class CaseController : Controller
    {

        aresdbEntities dbContext = new aresdbEntities();

        [_SessionControl]
        public ActionResult Index()
        {
            LoginInfo loginInfo = new LoginInfo();
            int firmId = 0;
            if (String.IsNullOrEmpty(loginInfo.GetFirmId()))
                return RedirectToAction("Login", "Login");
            firmId = Convert.ToInt32(loginInfo.GetFirmId());
            //List<Payment> incomeList = dbContext.Payments.Where(s => s.firmId == firmId && s.isIncome == true).ToList();
            CaseIndexModel model = new CaseIndexModel();
            model.allPayments = dbContext.Payments.Where(s => s.firmId == firmId).ToList();
            model.DailyIncome = model.allPayments.Where(s => s.isIncome == true && s.createdDate == DateTime.Now).Sum(s => s.amount);
            model.DailyOutcome = model.allPayments.Where(s => s.isIncome == false && s.createdDate == DateTime.Now).Sum(s => s.amount);
            model.MounthlyIncome = model.allPayments.Where(s => s.isIncome == true && s.createdDate.Month == DateTime.Now.Month).Sum(s => s.amount);
            model.MountlyOutcome = model.allPayments.Where(s => s.isIncome == false && s.createdDate.Month == DateTime.Now.Month).Sum(s => s.amount);
            return View(model);
        }

        [_SessionControl]
        public ActionResult Detail(int? id)
        {
            //if (id == 0 || id == null)
            //return Redirect("/Case/Index");
            List<Student> students = dbContext.Students.Where(s => s.id == id).ToList();
            //var paymentlist = dbContext.Payments.Where(s => s.firmId == id).ToList();
            double? total = 0;
            foreach (var item in students)
            {
                total -= item.totalAmount;
            }

            return View(students);


        }



    }
}