using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class CountryController : Controller
    {
        HMSEntities context = new HMSEntities();
        // GET: Country
        public ActionResult Index()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {
                if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var coutntry = context.Countries.ToList();
            return View(coutntry);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult Create()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Create(Country country)
        {
            if (ModelState.IsValid)
            {
                context.Countries.Add(country);
                context.SaveChanges();
                ViewBag.Message = "Add Successfully!";
                ModelState.Clear();
            }
            return View();
        }
        public ActionResult Edit(int? id)
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = context.Countries.SingleOrDefault(e => e.CountryId == id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                context.Entry(country).State = EntityState.Modified;
                context.SaveChanges();
                TempData["Message"] = "Update Successfully!";
                ModelState.Clear();
            }
            return View(country);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = context.Countries.SingleOrDefault(x => x.CountryId == id);
            context.Countries.Remove(country ?? throw new InvalidOperationException());
            context.SaveChanges();
            TempData["Message"] = "Delete Successfully!";
            return RedirectToAction("Index");
        }
    }
}