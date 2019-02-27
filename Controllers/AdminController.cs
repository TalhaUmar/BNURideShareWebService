using RideShareWebServices.DAC;
using RideShareWebServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RideShareWebServices.Controllers
{
    public class AdminController : Controller
    {
        
        public ActionResult AdminPage()
        {
            if (Session["CURRENT_ADMIN"] != null)
            {
                ViewBag.adminName = "Talha";
            }
            else
            {
                HttpCookie myCookie = Request.Cookies["info"];
                if (myCookie != null)
                {
                    string phone = myCookie.Values["PHONE_NUMBER"];

                    if (phone != null && phone == "03224228909")
                    {
                        Session.Add("CURRENT_ADMIN", "Talha");
                        ViewBag.userName = "Talha";
                        return RedirectToAction("AdminPage");
                    }
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            string message = "";
            
            if (model.Phone != null && model.Phone == "03224228909")
            {
                Session.Add("CURRENT_ADMIN", "Talha");
                HttpCookie cookie = new HttpCookie("info");
                cookie.Expires = DateTime.Today.AddDays(7);
                cookie.Values.Add("PHONE_NUMBER", "03224228909");
                Response.SetCookie(cookie);


                return Json(new { link = Url.Action("AdminPage", "Admin") });
            }
            else
            {
                message = "Phone is Incorrect";
                return Json(new { message });
            }

        }

        [HttpPost]
        public ActionResult InsertStudentId(Users model) 
        {
            model.PhoneStatus = 0;
            new UsersDAC().Insert(model);
            return RedirectToAction("AdminPage");
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            HttpCookie myCookie = Request.Cookies["info"];
            if (myCookie != null)
            {
                myCookie.Expires = DateTime.Now;
                Response.SetCookie(myCookie);
            }
            return RedirectToAction("AdminPage");
        }
    }
}