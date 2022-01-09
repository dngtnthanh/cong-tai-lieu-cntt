using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
using PagedList;
namespace WebDauTienVS2._0.Controllers
{
    public class TimKiemController : Controller
    {
        //
        // GET: /TimKiem/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult timKiem(int? id, string key, FormCollection collection)
        {
            ViewBag.KiemTra = "0";
            if (!string.IsNullOrEmpty(key))
            {
                ViewBag.key = key;
                NEWSDataContext context = new NEWSDataContext();
                var pages = from p in context.PageItems.OrderByDescending(p => p.ID_P)
                            .Where(p => p.Hide == true)
                            .Where(p => p.Name_File.Contains(key) || p.Contents.Contains(key))
                            .OrderBy(p => p.ID_P)
                            select p;
                ViewBag.Pages = pages;  
                //Đếm số lượng kết quả tìm kiếm
                var _slkq = pages.Count();
                ViewBag.slkq = _slkq;
                //Phân trang
                int pagesize = 2;
                int pageindex = id ?? 1;
                ViewBag.KiemTra = "1";
                return View(pages.ToPagedList(pageindex, pagesize));
            }
            else return View();                      
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult timKiemNEWS(int? id, string key, FormCollection collection)
        {
            ViewBag.KiemTra = "0";
            if (!string.IsNullOrEmpty(key))
            {
                ViewBag.key = key;
                NEWSDataContext context = new NEWSDataContext();
                var pages = from p in context.NEWS.OrderByDescending(p => p.ID_NEW)
                            .Where(p => p.Hide == true)
                            .Where(p => p.Title.Contains(key) || p.Contents.Contains(key))
                            .OrderBy(p => p.ID_NEW)
                            select p;
                ViewBag.Pages = pages;
                //Đếm số lượng kết quả tìm kiếm
                var _slkq = pages.Count();
                ViewBag.slkq = _slkq;
                //Phân trang
                int pagesize = 2;
                int pageindex = id ?? 1;
                ViewBag.KiemTra = "1";
                return View(pages.ToPagedList(pageindex, pagesize));
            }
            else return View();
        }
    }
}
