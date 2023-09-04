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
    public class HotelController : Controller
    {
        HMSEntities context = new HMSEntities();
        // GET: Hotel
        public ActionResult Index()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {
                if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var hotel = context.Hotels.Include("Country").ToList();
            return View(hotel);
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
                ViewBag.Country = context.Countries.ToList();
            return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Create(Hotel hotel)
        {
            
            if (ModelState.IsValid)
            {
                context.Hotels.Add(hotel);
                context.SaveChanges();
                ViewBag.Message = "Add Successfully!";
                ModelState.Clear();
            }
            ViewBag.Country = context.Countries.ToList();
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

            var hotel = context.Hotels.SingleOrDefault(e => e.HotelId == id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country = context.Countries.ToList();
            return View(hotel);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Edit(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                context.Entry(hotel).State = EntityState.Modified;
                context.SaveChanges();
                TempData["Message"] = "Update Successfully!";
                ModelState.Clear();
                return RedirectToAction("Index");
            }
            ViewBag.Country = context.Countries.ToList();
            return View(hotel);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var hotel = context.Hotels.SingleOrDefault(x => x.HotelId == id);
            context.Hotels.Remove(hotel ?? throw new InvalidOperationException());
            context.SaveChanges();
            TempData["Message"] = "Delete Successfully!";
            return RedirectToAction("Index");
        }
    }
}