using HotelManagementSystem.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace HotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        HMSEntities context = new HMSEntities();
        // GET: Room
        public ActionResult Index()
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {

                if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var room = context.Rooms.ToList();
            return View(room);
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
                ViewBag.Hotel = context.Hotels.ToList();
            return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file,Room room)
        {
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                string _fileName = DateTime.Now.ToString("yyyymmssff") + fileName;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), _fileName);
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".gif" || extension.ToLower() == ".png")
                {

                    room.Image = "~/Images/" + _fileName;
                    file.SaveAs(path);
                }
            }
            if (ModelState.IsValid)
                {
                    context.Rooms.Add(room);
                    context.SaveChanges();
                    ViewBag.Message = "Add Successfully!";
                    ModelState.Clear();
                }
            ViewBag.Hotel = context.Hotels.ToList();
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

            var room = context.Rooms.SingleOrDefault(e => e.RoomId == id);
            if (room == null)
            {
                return HttpNotFound();
            }
            Session["ImagePath"] = room.Image;
            ViewBag.Hotel = context.Hotels.ToList();
            return View(room);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file,Room room)
        {
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                string _fileName = DateTime.Now.ToString("yyyymmssff") + fileName;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), _fileName);
                room.Image = "~/Images/" + _fileName;
                if (ModelState.IsValid)
                {
                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".gif" || extension.ToLower() == ".png")
                    {
                        context.Entry(room).State = EntityState.Modified;
                        context.SaveChanges();
                        file.SaveAs(path);
                        string oldImage = Request.MapPath(Session["ImagePath"].ToString());
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }
                        TempData["Message"] = "Update Successfully!";
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                }
            }
            else 
            {
                if (ModelState.IsValid)
                {
                    room.Image = Session["ImagePath"].ToString();
                    context.Entry(room).State = EntityState.Modified;
                    context.SaveChanges();
                    TempData["Message"] = "Update Successfully!";
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Hotel = context.Hotels.ToList();
            return View(room);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var room = context.Rooms.SingleOrDefault(x => x.RoomId == id);
            string oldImage = Request.MapPath(room.Image);
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            context.Rooms.Remove(room ?? throw new InvalidOperationException());
            context.SaveChanges();
            TempData["Message"] = "Delete Successfully!";
            return RedirectToAction("Index");
        }
        //[HttpPost]
        public ActionResult RoomSearch(FormCollection formCollection, int PageNumber=1)
        {
            var cities = context.Hotels.Select(c => new { c.City }).Distinct().ToList();
            ViewBag.City = new SelectList(cities, "City", "City");
            var classes = context.Rooms.Select(c => new { c.ClassName }).Distinct().ToList();
            ViewBag.Class = new SelectList(classes, "ClassName", "ClassName");
            if(formCollection.Count != 0)
            {
                Session["City"] = formCollection["City"];
                Session["RoomType"] = formCollection["RoomType"];
                Session["Members"] = formCollection["Members"];
                Session["FromDate"] = formCollection["FromDate"];
                Session["ToDate"] = formCollection["ToDate"];
            }
            string City = Session["City"].ToString();
            string RoomType = Session["RoomType"].ToString();
            int Members = Convert.ToInt32(Session["Members"].ToString()); 
            string FromDate = Session["FromDate"].ToString();
            string ToDate = Session["ToDate"].ToString();
            TempData["sdate"] = FromDate;
            TempData["edate"] = ToDate;
            string query = "select rm.*,h.hotelname from Room rm,Hotel h where not exists(select r.* from Reservation r where r.checkindate between '"+FromDate+"' and '"+ToDate+"'and  r.checkoutdate between '"+FromDate+"' and '"+ToDate+"' and r.roomid=rm.roomid) and rm.MaxGuest='"+Members+"' and rm.ClassName='"+RoomType+"' and h.City='"+City+"' order by rm.price";
            var room = context.Rooms.SqlQuery(query).ToList();
            ViewBag.TotalPages = Math.Ceiling(room.Count() / 3.0);
            room=room.Skip((PageNumber-1)*3).Take(3).ToList();
            //var room = context.Rooms.Include("Hotel").Where(e=> e.Hotel.CountryId==Country && e.ClassName==RoomType && e.MaxGuest==Members).ToList();
            return View(room);
        }
        public ActionResult Details(int? id)
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                var room = context.Rooms.SingleOrDefault(e => e.RoomId == id);
            ViewBag.Review=context.Reviews.Where(x=>x.RoomId==id).ToList();
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
    }
}