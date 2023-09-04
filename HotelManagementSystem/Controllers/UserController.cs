using HotelManagementSystem.Helper;
using HotelManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace HotelManagementSystem.Controllers
{
    public class UserController : Controller
    {
        HMSEntities context = new HMSEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User user)
        {
            if (user.Email!=null&&user.Password!=null)
            {
                string md5StringPassword = EnDePasswrd.GetMd5Hash(user.Password);
                var us = context.Users.Where(u=> u.Email.Equals(user.Email) && u.Password.Equals(md5StringPassword)).FirstOrDefault();
                if(us != null)
                {
                    Session["UserId"] = us.UserId.ToString();
                    Session["Email"] = us.Email.ToString();
                    Session["Role"] = us.Role.ToString();
                    if (us.Image != null)
                    {
                        Session["UserImage"] = us.Image.ToString();
                    }
                    else
                    {
                        Session["UserImage"] = "~/Images/dummy.png";
                    }
                    if (us.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Country");
                    }
                    if (us.Role == "User")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid User";
                    ModelState.Clear();
                    return View(user);
                }
            }
            return View(user);
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(HttpPostedFileBase file, User user)
        {
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                string _fileName = DateTime.Now.ToString("yyyymmssff") + fileName;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), _fileName);
                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".gif" || extension.ToLower() == ".png")
                {

                    user.Image = "~/Images/" + _fileName;
                    file.SaveAs(path);
                }
            }
            if (ModelState.IsValid)
            {
                user.Password = EnDePasswrd.GetMd5Hash(user.Password);
                user.Role = "User";
                context.Users.Add(user);
                context.SaveChanges();
                ViewBag.Message = "SignUp Successfully!";
                ModelState.Clear();
                return View();
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login","User");
        }
        public ActionResult AdminProfile()
        {
            if (Session["Email"]!=null && Session["Role"].ToString() == "Admin")
            {
                string email = Session["Email"].ToString();
                var admin = context.Users.SingleOrDefault(e => e.Email == email);
                return View(admin);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        public ActionResult AdminEdit(int? id)
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = context.Users.SingleOrDefault(e => e.UserId == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult AdminEdit(User user)
        {
           
            var userdata = context.Users.Where(x => x.UserId == user.UserId).SingleOrDefault();
            if (user.Password == null)
            {
                user.Password = userdata.Password;
            }
            context.Users.AddOrUpdate(user);
            context.SaveChanges();
            ViewBag.Message = "Update Successfully!";
            ModelState.Clear();
            return RedirectToAction("AdminProfile");
        }
        public ActionResult UserProfile()
        {
            string email = Session["Email"].ToString();
            var user = context.Users.SingleOrDefault(e => e.Email == email);
            return View(user);
        }
        public ActionResult UserEdit(int? id)
        {
            if (Session["Email"] != null && Session["Role"].ToString() == "User")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = context.Users.SingleOrDefault(e => e.UserId == id);
            if (user == null)
            {
                return HttpNotFound();
            }
            Session["UserImagePath"] = user.Image;
            return View(user);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }
        [HttpPost]
        public ActionResult UserEdit(HttpPostedFileBase file, User user)
        {
            if (file != null)
            {
                string fileName = Path.GetFileName(file.FileName);
                string _fileName = DateTime.Now.ToString("yyyymmssff") + fileName;
                string extension = Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Images/"), _fileName);

                if (extension.ToLower() == ".jpg" || extension.ToLower() == ".gif" || extension.ToLower() == ".png")
                {
                    // Delete the previous image if it exists
                    if (Session["UserImage"] != null)
                    {
                        string oldImagePath = Server.MapPath(Session["UserImage"].ToString());
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    user.Image = "~/Images/" + _fileName;
                    file.SaveAs(path);
                    Session["UserImage"] = "~/Images/" + _fileName;
                }
            }
            else if (user.Image == null)
            {
                // User did not upload a new image and did not delete the existing one
                // Handle the logic accordingly, e.g., keep the existing image
                user.Image = Session["UserImage"]?.ToString();
            }

            var userdata = context.Users.Where(x => x.UserId == user.UserId).SingleOrDefault();
            if (user.Password == null)
            {
                user.Password = userdata.Password;
            }
            context.Users.AddOrUpdate(user);
            context.SaveChanges();
            ViewBag.Message = "Update Successfully!";
            ModelState.Clear();
            return RedirectToAction("UserProfile");
        }

    }
}