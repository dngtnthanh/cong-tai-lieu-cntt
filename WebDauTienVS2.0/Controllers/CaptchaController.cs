using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
using Newtonsoft.Json;
using System.Runtime.Serialization;
public static class ReCaptcha { }
namespace WebDauTienVS2._0.Controllers
{
    public class CaptchaController : Controller
    {
        //
        // GET: /Captcha/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult FormSubmit(int id)
        {
            ViewData["Message"] = "Xin hãy nhập reCaptcha";
            return View("Index");
        }
        [HttpPost]
        public ActionResult FormSubmit()
        {
            //Validate Google recaptcha below
            var response = Request["g-recaptcha-response"];
            string secretKey = "6Le6gdccAAAAAM43NTj_w0p-cJvcEYh3V4NOK7g2";
            var client = new WebClient();      
            ViewData["Message"] = "Google reCaptcha validation success";
            //Here I am returning to Index view:
            return View("Index");
        }
    }
}
