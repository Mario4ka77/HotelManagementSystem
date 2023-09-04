using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class ReviewController : Controller
    {
        HMSEntities context = new HMSEntities();
        // GET: Review
        public ActionResult Index()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                return RedirectToAction("Booking", "Reservation");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult Add()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                Review review = new Review();
            review.RoomId = Convert.ToInt32(TempData["RoomId"]);
            return View(review);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Add(Review review)
        {
            review.CurrentDate = DateTime.Now;
            review.UserId = Convert.ToInt32(Session["UserId"]);
            context.Reviews.Add(review);
            context.SaveChanges();
            ViewBag.Message = "Add Successfully!";
            ModelState.Clear();
            return RedirectToAction("Booking", "Reservation");
        }
    }
}