using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
using System.IO;
using PagedList;

namespace WebDauTienVS2._0.Controllers
{
    public class NEWSController : Controller
    {
        // GET: /NEWS/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add_News()
        {
            NEWSDataContext context = new NEWSDataContext();
            var temp = Session["mn"];             //Biến tạm temp chứa 'mn' để dành ép kiểu sang int phía dưới
            int _mn = int.Parse(temp.ToString()); //Biến _mn chứa số id danh mục tin tức
            
            Menus_NEW mn_new = context.Menus_NEWs.Single(m => m.ID_MN_NEW == _mn);
            ViewBag.danhmuc = mn_new.Lable;
            var NEWS = from m in context.NEWS.OrderBy(m => m.ID_NEW) select m;
            ViewBag.parent = NEWS.First().ID_NEW;
            ViewBag.idnew = (context.NEWS.Max(n => n.ID_NEW) + 1).ToString();
            ViewBag.parents = NEWS;
            return View();
        }
        [ValidateInput(false)]
        public ActionResult Add_News(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            var temp = Session["mn"];             //Biến tạm temp chứa 'mn' để dành ép kiểu sang int phía dưới
            int _mn = int.Parse(temp.ToString()); //Biến _mn chứa số id danh mục tin tức
            string _Title = collection.Get("Title");
            string _Sumary = collection.Get("Sumary");
            var CK = collection.Get("editor1");
            long idmn = context.NEWS.Max(n => n.ID_NEW) + 1;
            NEWS news = new NEWS();
            news.ID_NEW = idmn;
            news.Title = _Title;
            news.Sumary = _Sumary;
            news.Contents = CK;
            ViewBag.context = CK;
            news.image = "unnamed.jpg";         //Đặt ảnh mặc định
            news.CreaData = DateTime.Now;       //Lấy ngày giờ hiện tại thêm vào bảng tin
            news.ID_MN_NEW = _mn;               //Gán danh mục cho tin tức
            news.Username = Session["Username"].ToString();
            context.NEWS.InsertOnSubmit(news);
            context.SubmitChanges();
            ViewBag.idnew = (context.NEWS.Max(n => n.ID_NEW) + 1).ToString();
            return View();
        }
        public ActionResult DanhSachNews(int ?id, int ?mn)
        {
            if (mn != null)    Session.Add("mn", mn);
            
            NEWSDataContext context = new NEWSDataContext();

            var news = from m in context.NEWS.OrderByDescending(m => m.ID_NEW)
                       where (m.ID_MN_NEW.ToString() == Session["mn"].ToString())
                       select m;
            ViewBag.news = news;
            int pagesize = 2;
            int pageindex = id ?? 1;
            return View(news.ToPagedList(pagesize, pageindex));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult View_TinTuc(int? id = 1)
        {
            NEWSDataContext context = new NEWSDataContext();
            var page = context.NEWS.Single(p => p.ID_NEW == id);
            var links = from p in context.NEWS.OrderBy(p => p.ID_NEW)
                        .Where(p => p.ID_NEW > id).Take(10)
                        select p;
            ViewBag.links = links;
            return View(page);
        }
        public ActionResult Edit_TieuDe(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var nEWS = context.NEWS.Single(m => m.ID_NEW == id);
            return View(nEWS);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit_TieuDe(FormCollection collection, int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            string lable = collection.Get("Title");
            string _Tomtat = collection.Get("Sumary");
            NEWS nEWS = context.NEWS.Single(m => m.ID_NEW == id);
            nEWS.Title = lable;
            nEWS.Sumary = _Tomtat;
            nEWS.ModiData = DateTime.Now;
            context.SubmitChanges();
            return RedirectToAction("DanhSachNews");
        }
        public ActionResult Delete_TinTuc(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            NEWS nEWS = context.NEWS.Single(n => n.ID_NEW == id);
            context.NEWS.DeleteOnSubmit(nEWS);
            context.SubmitChanges();
            return RedirectToAction("DanhSachNews");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EditNews(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            NEWS nEWS = context.NEWS.Single(m => m.ID_NEW == id);
            ViewBag.context = nEWS.Contents;
            ViewBag.ModiData = null;
            ViewBag.CreaData = nEWS.CreaData;
            return View(nEWS);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        public ActionResult EditNews(FormCollection collection, int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            NEWS nEWS = context.NEWS.Single(m => m.ID_NEW == id);
            var CK = collection.Get("editor1");
            nEWS.Contents = CK;
            ViewBag.context = CK;
            nEWS.ModiData = DateTime.Now;
            ViewBag.ModiData = nEWS.ModiData;
            ViewBag.CreaData = nEWS.CreaData;
            context.SubmitChanges();
            return View();
        }
        public ActionResult CapNhatNews() {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult View_TinTuc_Home(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            NEWS page = context.NEWS.Single(p => p.ID_NEW == id);
            ViewBag.TieuDe = page.Title;
            return View(page);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Uploads()
        {
            ViewBag.message = "Chọn file cần lưu";
            return View();
        }
        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Uploads(HttpPostedFileBase file, int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            NEWS pi = context.NEWS.Single(n => n.ID_NEW == id);
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var id_new = pi.ID_NEW;
                // Khai báo biến fileName để lưu địa chỉ file trong forder
                var fileName = id_new + Path.GetExtension(file.FileName);     //đổi tên file giống với id của chính nó + giữ nguyên phần mở rộng
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/Uploads/NEWS"), fileName);
                file.SaveAs(path);
                pi.ModiData = DateTime.Now;      //Gán ngày giờ hiện tại vào time thay đổi tin
                pi.image = fileName;      //Lưu địa chỉ file = tên file đã đổi giống với IP_P.đuôi file
                pi.Hide = true;
                ViewBag.ThongBao = "Upload đã thành công";
                context.SubmitChanges();
            }
            // redirect back to the index action to show the form once again
            return View();

        }
        public ActionResult DanhSachNewsMenu(int ?id, int ?mn)
        {
            NEWSDataContext context = new NEWSDataContext();
            var news = from m in context.NEWS.OrderByDescending(m => m.ID_NEW) select m;
            ViewBag.news = news;
            ViewBag.news = news;
            int pagesize = 2;
            int pageindex = id ?? 1;
            return View(news.ToPagedList(pagesize, pageindex));
        }
        public ActionResult ViewTinTuc_danhMuc(FormCollection collection, int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var page = from p in context.NEWS where (p.Hide == true && p.ID_MN_NEW == id) select p;
            ViewBag.Pages = page;
            ViewBag.Page = collection.Get("ID_P");
            long _id = Convert.ToInt64(ViewBag.Page);
            var menus = from m in context.Menus_NEWs.OrderByDescending(m => m.ID_MN_NEW) where (m.Parent == _id) select m;
            ViewBag.menus = menus;
            return View();
        }
    }
}
