using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace HotelManagementSystem.Controllers
{
    public class ReservationController : Controller
    {
        HMSEntities context = new HMSEntities();
        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create(int ? id)
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                int userid = Convert.ToInt32(Session["UserId"].ToString());
            ViewBag.User = context.Users.Where(u=>u.UserId==userid).ToList();
            ViewBag.Room = context.Rooms.Where(u => u.RoomId == id).ToList();
            var rooms = context.Rooms.Where(u => u.RoomId == id).SingleOrDefault();
            Reservation reservation = new Reservation();
            reservation.ReservationDate = DateTime.Now;
            reservation.CheckInDate = Convert.ToDateTime(TempData["sdate"]);
            reservation.CheckOutDate = Convert.ToDateTime(TempData["edate"]);
            TimeSpan t = Convert.ToDateTime(TempData["edate"]) - Convert.ToDateTime(TempData["sdate"]);
            reservation.TotalPrice = Convert.ToInt32(t.TotalDays * (rooms.Price));
            return View(reservation);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Create(Reservation reservation)
        {
            reservation.Status = "Pending";
            context.Reservations.Add(reservation);
            context.SaveChanges();
            ViewBag.Message = "Add Successfully!";
            ModelState.Clear();
            return RedirectToAction("Booking","Reservation");
        }
        public ActionResult Booking()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                int UserId = Convert.ToInt32(Session["UserId"].ToString());   
            var bookings = context.Reservations.Where(r => r.UserId==UserId).ToList();
            return View(bookings);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult Bookings()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {
                var bookings=context.Reservations.OrderBy(r=>r.ReservationId).ToList();
            return View(bookings);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult Approve(int? id)
        {
            var bookings = context.Reservations.Where(r=>r.ReservationId==id).SingleOrDefault();
            var user = context.Users.Where(u=>u.UserId==bookings.UserId).SingleOrDefault();
            Reservation reservation=new Reservation();
            reservation.ReservationId = bookings.ReservationId;
            reservation.RoomId= bookings.RoomId;
            reservation.UserId = bookings.UserId;
            reservation.Description = bookings.Description;
            reservation.CheckInDate = bookings.CheckInDate;
            reservation.CheckOutDate = bookings.CheckOutDate;
            reservation.TotalPrice= bookings.TotalPrice;
            reservation.ReservationDate = bookings.ReservationDate;
            reservation.Status = "Approved";
            //context.Entry(reservation).State = EntityState.Modified;
            context.Reservations.AddOrUpdate(reservation);
            context.SaveChanges();
            try
            {
                var senderEmail = new MailAddress("bookme.hotelsystem@gmail.com", "Admin");
                var receiverEmail = new MailAddress(user.Email, "Receiver");
                var password = "ledtabjwrfnbqnnu";
                var sub = "Hotel Booking Confirmation";
                var body = "Hello " + user.FirstName + "!\n\n" + " Your  " + reservation.Room + " reservation is confirmed.\n Please transfer "
                    + reservation.TotalPrice + " lv. to our bank account.\n IBAN: BG18RZBB91550123456789\n We are waiting for you on " + reservation.CheckInDate + " \n Thank you for choosing us.";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = sub,
                    Body = body.Replace("\n", "<br />"),
                    IsBodyHtml = true
                   
                })
                {
                    smtp.Send(mess);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex;
            }
            return RedirectToAction("Bookings");
        }
        public ActionResult Cencel(int? id)
        {
            var bookings = context.Reservations.Where(r => r.ReservationId == id).SingleOrDefault();
            Reservation reservation = new Reservation();
            reservation.ReservationId = bookings.ReservationId;
            reservation.RoomId = bookings.RoomId;
            reservation.UserId = bookings.UserId;
            reservation.Description = bookings.Description;
            reservation.CheckInDate = bookings.CheckInDate;
            reservation.CheckOutDate = bookings.CheckOutDate;
            reservation.TotalPrice = bookings.TotalPrice;
            reservation.ReservationDate = bookings.ReservationDate;
            reservation.Status = "Canceled";
            //context.Entry(reservation).State = EntityState.Modified;
            context.Reservations.AddOrUpdate(reservation);
            context.SaveChanges();
            return RedirectToAction("Bookings");
        }
        public ActionResult Review(int? id)
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                TempData["RoomId"] = id;
                return RedirectToAction("Add","Review");
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
    }
}