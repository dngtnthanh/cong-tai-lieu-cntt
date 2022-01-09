using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
namespace WebDauTienVS2._0.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(){
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection collection) {
            
            NEWSDataContext context = new NEWSDataContext();
            string us = collection.Get("Username");
            string p = Security.MD5(collection.Get("Password"));
            Acount ac = context.Acounts.SingleOrDefault(a => a.Username == us && a.Password == p && a.Lock == false);
            if (ac != null)
            {
                Session["Username"] = us;
                Session["Permission"] = ac.Permission;
                Session["Avatar"] = ac.Avatar;
                return RedirectToAction("Index", "admin");
            }
            else {
                ViewBag.ThongBao = "Đăng nhập thất bại";
            }
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LoginHome() {

            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LoginHome(FormCollection collection) {
            NEWSDataContext context = new NEWSDataContext();
            string us = collection.Get("Username");
            string p = Security.MD5(collection.Get("Password"));
            Acount ac = context.Acounts.SingleOrDefault(m => m.Username == us && m.Password == p && m.Lock == false && m.Permission == 3);
            if (ac != null) {
                Session["Username"] = us;
                Session["Permission"] = ac.Permission;
                Session["Avarta"] = ac.Avatar;
                Session["Fullname"] = ac.Fullname;
                Session["Likes"] = ac.Likes;
                Session["Dislikes"] = ac.Dislikes;
                Session["Cups"] = ac.Cups;
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ThongBao = "Đăng nhập thất bại";
            return View();
        }    
        public ActionResult DangXuat() {
            Session["Username"] = "Login";
            Session["Fullname"] = "";
            Session["Permission"] = 0;
            Session["Avatar"] = "";
            Session["Email"] = "";        
            return RedirectToAction("Index", "Home");
        }
    }
}
