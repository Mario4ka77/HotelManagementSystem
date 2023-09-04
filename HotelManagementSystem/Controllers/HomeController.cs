using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        HMSEntities context = new HMSEntities();
        public ActionResult Index()
        {
            var cities= context.Hotels.Select(c => new { c.City }).Distinct().ToList();
            ViewBag.City = new SelectList(cities, "City", "City");
            var classes = context.Rooms.Select(c => new {c.ClassName}).Distinct().ToList();
            ViewBag.Class = new SelectList(classes,"ClassName","ClassName");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Blog()
        {
            ViewBag.Message = "Your blog page.";

            return View();
        }
    }
}