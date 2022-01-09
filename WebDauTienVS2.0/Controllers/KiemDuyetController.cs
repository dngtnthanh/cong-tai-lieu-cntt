using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
using System.IO;
namespace WebDauTienVS2._0.Controllers
{
    public class KiemDuyetController : Controller
    {
        //
        // GET: /KiemDuyet/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult KiemDuyetTaiLieu()
        {
            NEWSDataContext context = new NEWSDataContext();
            var page = from p in context.PageItems.OrderByDescending(n => n.ID_P) where (p.Hide == false) select p;
            ViewBag.Pages = page;
            Session["ThongKeDuyet"] = page.Count();
            long id = Convert.ToInt64(ViewBag.Page);
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult View_TaiLieu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var _Page = context.PageItems.Single(m => m.ID_P == id);
            ViewBag.soTaiLieu = id;
            return View(_Page);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Delete_TaiLieu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            PageItem page = context.PageItems.Single(n => n.ID_P == id);
            context.PageItems.DeleteOnSubmit(page);
            context.SubmitChanges();
            return RedirectToAction("KiemDuyetTaiLieu");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit_TaiLieu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menu = context.PageItems.Single(m => m.ID_P == id);
            return View(menu);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit_Page(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            int id = Convert.ToInt32(collection.Get("ID_P"));
            string lable = collection.Get("Title");
            PageItem menu = context.PageItems.Single(m => m.ID_P == id);
            menu.Title = lable;
            context.SubmitChanges();
            return RedirectToAction("KiemDuyetTaiLieu");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Duyet_TaiLieu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            PageItem page = context.PageItems.Single(n => n.ID_P == id);
            page.Hide = true;
            context.SubmitChanges();
            return RedirectToAction("KiemDuyetTaiLieu");
        }
    }
}
