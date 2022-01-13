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
    public class adminController : Controller
    {
        // GET: /admin/
        public ActionResult Index()
        {
            //Kiểm tra đăng nhập. Nếu chưa, trả về Login
            if (Session["Username"].ToString() == "Login" || Session["Permission"].ToString() == "3") 
                return RedirectToAction("Index", "Login");
                NEWSDataContext context = new NEWSDataContext();
                var menus = (from m in context.Menus.OrderBy(m => m.ID_MN)
                             where (m.Parent == 0)
                             select m);
            //Phần này xuất thông tin số tài liệu chưa duyệt vào biến tc "ThongKeDuyet" load vào layout admin
                var ThongKeDuyet = from n in context.PageItems.OrderByDescending(n => n.ID_P)
                                   where (n.Hide == false)
                                   select n;            
                ViewBag.Menus = menus;
                Session["ThongKeDuyet"] = ThongKeDuyet.Count(); 
                return View();   
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult menuList()
        {
            NEWSDataContext context = new NEWSDataContext();
            var parents = from p in context.Menus where (p.Parent == 0) select p;
            ViewBag.parents = parents;
            ViewBag.parent = parents.First().ID_MN;
            long id = ViewBag.parent;
            var menus = from m in context.Menus.OrderBy(m => m.ID_MN) where (m.Parent == id) select m;
            ViewBag.menus = menus;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult menulist(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            var parents = from p in context.Menus where (p.Parent == 0) select p;
            ViewBag.parents = parents;
            ViewBag.parent = collection.Get("ID_MN");
            long id = Convert.ToInt64(ViewBag.parent);
            var menus = from m in context.Menus.OrderBy(m => m.ID_MN) where (m.Parent == id) select m;
            ViewBag.menus = menus;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ViewMenu(FormCollection collection, int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var page = from p in context.PageItems where (p.Hide == true && p.ID_MN == id) select p;
            ViewBag.Pages = page;
            ViewBag.Page = collection.Get("ID_P");
            long _id = Convert.ToInt64(ViewBag.Page);
            var menus = from m in context.Menus.OrderBy(m => m.ID_MN) where (m.Parent == _id) select m;
            ViewBag.menus = menus;
            return View();
        }
        public ActionResult SettingUser()
        {
            NEWSDataContext context = new NEWSDataContext();
            var Account = (from m in context.Acounts.OrderBy(m => m.Username)
                           select m);
            ViewBag.accout = Account;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AddMenu()
        {
            ViewBag.Message = "Nhập thông tin";

            NEWSDataContext context = new NEWSDataContext();
            var parents = from m in context.Menus where (m.Parent == 0) select m;
            ViewBag.parent = parents.First().ID_MN;
            ViewBag.idnew = (context.Menus.Max(n => n.ID_MN) + 1).ToString();
            ViewBag.parents = parents;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public string AddMenu(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            string error = "";
            string result = "";
            long id = 0;
            if (!string.IsNullOrEmpty(collection.Get("ID_MN"))) 
            {
                id = Convert.ToInt64(collection.Get("ID_MN"));
                if (context.Menus.SingleOrDefault(m=>m.ID_MN == id) != null ) error +="ID_MN đã tồn tại<br/>";  
            }
            else error += "Chưa nhập ID_MN<br/>";
            string lable = "";
            if (!string.IsNullOrEmpty(collection.Get("Lable")))
                lable = collection.Get("Lable");
            else
                error += "Chưa nhập Lable<br/>";
            byte pos = 0;
            if (!string.IsNullOrEmpty(collection.Get("Pos")))
            {
                if (!byte.TryParse(collection.Get("Pos"), out pos)) error += "Pos phải là số <br/>";
            }
            else
                error += "Chưa nhập Pos<br/>";
            string url = "";
            if (!string.IsNullOrEmpty(collection.Get("UrLink")))
                url = collection.Get("UrLink");
            else
                error += "Chưa nhập Url<br/>";

            long parent = 0;
            if (!string.IsNullOrEmpty(collection.Get("ID_Parent")))
                parent = Convert.ToInt64(collection.Get("ID_Parent"));
            else
                error += "Chưa chọn menu cha<br/>";
            if (!string.IsNullOrEmpty(error))
                result = error;
            else
            {
                //Tiến hành lưu menu
                Menus menu = new Menus();
                menu.ID_MN = id;
                menu.Parent = parent;
                menu.Lable = lable;
                menu.Pos = pos;
                menu.Parent = parent;
                menu.UrLink = url;
                menu.Username = "duongthanh";
                context.Menus.InsertOnSubmit(menu);
                context.SubmitChanges();
                result = "Lưu thành công, bạn có thể thêm nữa";
            }                       

            var parents = from m in context.Menus where (m.Parent == 0) select m;
            ViewBag.parent = parent;
            ViewBag.idnew = (context.Menus.Max(n => n.ID_MN) + 1).ToString();
            ViewBag.parents = parents;
            return result;
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult EditMenu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menu = context.Menus.Single(m => m.ID_MN == id);
            ViewBag.parent = menu.Parent;
            var parents = from m in context.Menus where (m.Parent == 0) select m;
            ViewBag.parents = parents;
            return View(menu);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditMenu(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            int id = Convert.ToInt32(collection.Get("ID_MN"));
            string lable = collection.Get("Lable");
            byte pos = Convert.ToByte(collection.Get("pos"));
            long parent = Convert.ToInt64(collection.Get("ID_Parent"));
            string url = collection.Get("UrLink");
            Menus menu = context.Menus.Single(m => m.ID_MN == id);
            menu.Lable = lable;
            menu.Pos = pos;
            menu.Parent = parent;
            menu.UrLink = url;
            context.SubmitChanges();
            return RedirectToAction("MenuList");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeleteMenu(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            Menus menu = context.Menus.Single(n => n.ID_MN == id);
            context.Menus.DeleteOnSubmit(menu);
            context.SubmitChanges();
            return RedirectToAction("MenuList");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DeletePage(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            PageItem page = context.PageItems.Single(n => n.ID_P == id);
            context.PageItems.DeleteOnSubmit(page);
            context.SubmitChanges();
            return RedirectToAction("ViewPageltems");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Uploads()
        {
            NEWSDataContext context = new NEWSDataContext();            
            int _mntl = int.Parse(Session["mntl"].ToString()); //Biến mntl chứa số id danh mục tài liệu
            Menus mn = context.Menus.Single(m => m.ID_MN == _mntl); //Biến này để lấy tên danh mục
            ViewBag.mntl = mn.Lable;    //Lấy tên danh mục
            ViewBag.message = "Chọn file cần lưu";
            return View();
        }
        [HttpPost]
        public ActionResult Uploads(HttpPostedFileBase file, FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                var temp = Session["mntl"];             //Biến tạm temp chứa 'mntl' để dành ép kiểu sang int phía dưới
                int _mntl = int.Parse(temp.ToString()); //Biến mntl chứa số id danh mục tài liệu
                PageItem pi = new PageItem();
                var id_p = context.PageItems.Max(m => m.ID_P) + 1;
                pi.Name_File = file.FileName;       //Lưu tên file vào csdl

                // Khai báo biến fileName để lưu địa chỉ file trong forder
                var fileName = id_p + Path.GetExtension(file.FileName);     //đổi tên file giống với id của chính nó + giữ nguyên phần mở rộng
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                file.SaveAs(path);

                pi.Title = collection.Get("Title");
                pi.Sumary = collection.Get("Sumary");
                pi.Username = Session["Username"].ToString();

                pi.ID_P = id_p;
                pi.ID_File = fileName;      //Lưu địa chỉ file = tên file đã đổi giống với IP_P.đuôi file
                pi.Hide = true;
                ViewBag.ThongBao = "Upload đã thành công";
                pi.ID_MN = _mntl;

                context.PageItems.InsertOnSubmit(pi);
                context.SubmitChanges();
            }
            // redirect back to the index action to show the form once again
            return View();

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ViewPageltems(FormCollection collection, int? id, int? mntl)
        {

            if (mntl != null) Session.Add("mntl", mntl);
            NEWSDataContext context = new NEWSDataContext();
            var _taiLieu = from m in context.PageItems.OrderByDescending(m => m.ID_MN)
                           where (m.ID_MN.ToString() == Session["mntl"].ToString() && m.Hide == true)
                           select m;

            ViewBag.Pages = _taiLieu;

            //NEWSDataContext context = new NEWSDataContext();
            //var page = from p in context.PageItems.OrderByDescending(m=> m.ID_P) where (p.Hide == true) select p;
            //ViewBag.Pages = page;
            //ViewBag.Page = collection.Get("ID_P");
            //long _id = Convert.ToInt64(ViewBag.Page);
            //var menus = from m in context.Menus.OrderBy(m => m.ID_MN) where (m.Parent == id) select m;
            //ViewBag.menus = menus;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ViewTaiLieu_Admin_Page(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var _Page = context.PageItems.Single(m => m.ID_P == id);
            return View(_Page);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit_Page(int id)
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
            return RedirectToAction("ViewPageltems");
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit_Avarta(int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var TK = context.Acounts.Single(m => m.ID_User == id);
            ViewBag.message = "Chọn file cần lưu";
            ViewBag.Username = TK.Username;
            ViewBag.loadAnh = TK.Avatar;
            ViewBag.FullName = TK.Fullname;
            ViewBag.Email = TK.Email;
            ViewBag.Password = TK.Password;
            return View(TK);
        }
        [HttpPost]
        public ActionResult Edit_Avarta(HttpPostedFileBase file, int id)
        {
            NEWSDataContext context = new NEWSDataContext();
            Acount TK = context.Acounts.Single(n => n.ID_User == id);
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // Khai báo biến fileName để lưu địa chỉ file trong forder
                string fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/Uploads/Avarta"), fileName);
                file.SaveAs(path);
                TK.Avatar = fileName;         
                ViewBag.ThongBao = "Upload đã thành công";
                context.SubmitChanges();
            }
            ViewBag.loadAnh = TK.Avatar;
            // redirect back to the index action to show the form once again
            return View();

        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ThayDoiQuyenHT(int id)
        {
            if (Session["Permission"].ToString() == "1")
            {
                NEWSDataContext context = new NEWSDataContext();
                Acount TK = context.Acounts.Single(n => n.ID_User == id);
                if (TK.Lock == false && TK.Username != "Administrator") //Chặn Administrator khóa quyền của chính mình
                {
                    TK.Lock = true;       //Khóa tài khoản
                    ViewBag.ThongBao = "Đã khóa tài khoản này";
                }
                else
                {
                    if (TK.Lock == true)
                    {
                        TK.Lock = false;      //Bật lại tài khoản 
                        ViewBag.ThongBao = "Đã mở khóa tài khoản";
                    }
                }
                context.SubmitChanges();
            }
            else {
                ViewBag.ThongBao = "Bạn không có quyền thay đổi, Hãy liên hệ quản trị viên";
            }
            
            return RedirectToAction("SettingUser");
        }
    }
}
