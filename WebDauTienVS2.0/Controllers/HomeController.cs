using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using WebDauTienVS2._0.Models;
using PagedList;
using System.Net;
using QRCoder;
namespace WebDauTienVS2._0.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index(int? id)
        {
            MenuProvider menuDrop = new MenuProvider();
            Statistical boDiem = new Statistical();
            NEWSDataContext context = new NEWSDataContext();
            var pages = from p in context.NEWS.OrderByDescending(p => p.ID_NEW) select p;
            ViewBag.boDiem = boDiem.Traffic.ToString();
            int pagesize = 4;
            int pageindex = id ?? 1;            
            return View(pages.ToPagedList(pageindex, pagesize));
        }
        public ActionResult veMIT() {
            return View();
        }
        public ActionResult spToan() {
            return View();
        }
        public ActionResult GioiThieu() {
            return View();
        }
        public ActionResult cntt() {
            return View();
        }
        public ActionResult boMon() {
            return View();
        }  
        public ActionResult TaiLieu(int ?id, int ?mntl)
        {
            if (mntl != null) Session.Add("mntl", mntl);
            NEWSDataContext context = new NEWSDataContext();
            var _taiLieu = from m in context.PageItems.OrderByDescending(m => m.ID_MN)
                           where (m.ID_MN.ToString() == Session["mntl"].ToString() && m.Hide == true)
                           select m;
            
            ViewBag.taiLieu = _taiLieu;
            int pagesize = 2;
            int pageindex = id ?? 1;
            return View(_taiLieu.ToPagedList(pagesize, pageindex));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ViewMenu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menu = context.Menus.Single(m => m.ID_MN == id);
            return View(menu);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ViewTaiLieu_Page(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var _Page = context.PageItems.Single(m => m.ID_P == id);
            return View(_Page);
        }     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ListPage(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            var page = from p in context.PageItems  select p;
            ViewBag.Pages = page;
            ViewBag.Page = collection.Get("ID_P");
            long id = Convert.ToInt64(ViewBag.Page);
            var menus = from m in context.Menus.OrderBy(m => m.ID_MN) where (m.Parent == id) select m;
            ViewBag.menus = menus;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DSTaiLieu(FormCollection collection, int? _id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var page_ = from p in context.PageItems.OrderByDescending(p => p.ID_P) where (p.Hide == true) select p;
            ViewBag.Pages = page_;
            ViewBag.Page = collection.Get("ID_P");
            long id = Convert.ToInt64(ViewBag.Page);
            var menus = from m in context.Menus.OrderBy(m => m.ID_MN) where (m.Parent == id) select m;
            ViewBag.menus = menus;
              // var pages = from p in context.PageItems.OrderByDescending(p => p.ID_P) select p;
               int pagesize = 2;
               int pageindex = _id ?? 1;
            return View(page_.ToPagedList(pageindex, pagesize));          
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DongGopTaiLieu()
        {
            if (Session["Permission"].ToString() != "3") {              //Kiểm tra nếu chưa đăng nhập, thì yêu cầu đăng nhập
                return RedirectToAction("LoginHome", "Login");
            }
            NEWSDataContext context = new NEWSDataContext();
            var temp = Session["mntl"];             //Biến tạm temp chứa 'mntl' để dành ép kiểu sang int phía dưới
            int _mntl = int.Parse(temp.ToString()); //Biến mntl chứa số id danh mục tài liệu
            Menus mn = context.Menus.Single(m => m.ID_MN == _mntl);
            ViewBag.MaSoTL = null;
            ViewBag.message = "Chọn file cần lưu";
            ViewBag.mntl = mn.Lable;
            return View();
        }
        [HttpPost]
        public ActionResult DongGopTaiLieu(HttpPostedFileBase file, FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            long id_p = context.PageItems.Max(n => n.ID_P) + 1;
            // extract only the filename
            
            if (file != null && file.ContentLength > 0 )   // Khi file có giá trị thì thực hiện
            {
                var _fileName = Path.GetFileName(file.FileName);    //hàm if bên dưới xác định các định dạng file được hỗ trợ khi User tải lên
                if (Path.GetExtension(_fileName).ToLower().Contains("pdf") == true || Path.GetExtension(_fileName).ToLower().Contains("doc") == true || Path.GetExtension(_fileName).ToLower().Contains("docx") == true || Path.GetExtension(_fileName).ToLower().Contains("jpg") == true || Path.GetExtension(_fileName).ToLower().Contains("png") == true || Path.GetExtension(_fileName).ToLower().Contains("ppt") == true || Path.GetExtension(_fileName).ToLower().Contains("pptx") == true)
                {
                    PageItem pi = new PageItem();
                    var temp = Session["mntl"];             //Biến tạm temp chứa 'mntl' để dành ép kiểu sang int phía dưới
                    int _mntl = int.Parse(temp.ToString()); //Biến mntl chứa số id danh mục tài liệu
                    Menus mn = context.Menus.Single(m => m.ID_MN == _mntl);
                    pi.ID_P = id_p;
                    //Khai báo biến _FileName để lưu tên file
                    pi.Name_File = file.FileName;
                    pi.Title = collection.Get("Title");
                    pi.Sumary = collection.Get("Sumary");
                    pi.Username = Session["Username"].ToString();
                    pi.ID_MN = _mntl;      //Lưu danh mục cho tài liệu
                    pi.CreaData = DateTime.Now;
                    ViewBag.mntl = mn.Lable;

                    var fileName = id_p + Path.GetExtension(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    file.SaveAs(path);
                    pi.Hide = false;        //Trạng thái chưa duyệt tài liệu
                    pi.ID_File = fileName;      //Lưu tên file vào csdl trước khi file đổi thành ID_file để tránh ghi đè file trong thư mục
                    ViewBag.ThongBao = "Upload đã thành công";
                    //<i class="fas fa-exclamation-triangle">   => Icon chú ý
                    ViewBag.ChuY = "<i class='fas fa-exclamation-triangle'></i> CHÚ Ý: Tài liệu của bạn đang được kiểm duyệt, vui lòng quay lại sau để kiểm tra";
                    ViewBag.MaSoTL = "Mã số tài liệu: ";
                    ViewBag.ip_p = id_p;
                    ViewBag.CamOn = "Chân thành cảm ơn bạn đã đóng góp tài liệu";
                    context.PageItems.InsertOnSubmit(pi);
                    context.SubmitChanges();
                }
                else
                {
                    ViewBag.ThongBao = "Tải lên không thành công, định dạng File chưa được hỗ trợ";
                    return View();
                }

            }
            // redirect back to the index action to show the form once again
            return View();
            }
        public ActionResult QRcode(int id) {
            NEWSDataContext context = new NEWSDataContext();
            PageItem qr = context.PageItems.Single(m => m.ID_P == id);

            return View();
        }
    }
}
