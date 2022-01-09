using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
using System.IO;
namespace WebDauTienVS2._0.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add_TaiKhoan()
        {
            if (Session["Permission"].ToString() == "1")
            {
                NEWSDataContext context = new NEWSDataContext();
                var TK = from m in context.Acounts.OrderBy(m => m.ID_User) select m;
                ViewBag.parent = TK.First().ID_User;
                ViewBag.idnew = (context.Acounts.Max(n => n.ID_User) + 1).ToString();
                ViewBag.parents = TK;
            }
            else {
                ViewBag.ThongBao = "Bạn không có quyền thêm mới tài khoản, vui lòng liên hệ quản trị viên";
            }
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add_TaiKhoan(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext(); 
            string _Username = collection.Get("Username");
           // string _Password = SHA256(collection.Get("Password"));            //  Mã hóa sha256
           // string _Password2 = SHA256(collection.Get("Password2"));
            string _Password = Security.MD5(collection.Get("Password"));        //Mã hóa MD5
            string _Password2 = Security.MD5(collection.Get("Password2"));
            string _Email = collection.Get("Email");
            long _id = context.Acounts.Max(n => n.ID_User) + 1;
            Acount TK = new Acount();
            if (_Password == _Password2)
            {
                TK.ID_User = _id;
                TK.Username = _Username;
                TK.Email = _Email;
                TK.Password = _Password;
                TK.Active = false;
                TK.Permission = 2;
                TK.Lock = false;
                TK.Avatar = "images.jpg";     //Gán ảnh mặc định
                TK.Introduce_Yourself = "";
                TK.Join = DateTime.Now;
                TK.Likes = 0;
                TK.Cups = 0;
                TK.Dislikes = 0;
                TK.Confirm = false;
                context.Acounts.InsertOnSubmit(TK);
                context.SubmitChanges();
                ViewBag.ThongBao = "Thêm tài khoản thành công";
            }
            else {
                if (_Password != _Password2) {
                    ViewBag.ThongBao = "Mật khẩu không trùng khớp, vui lòng nhập lại";
                }
            }            
            ViewBag.idnew = (context.PageItems.Max(n => n.ID_P) + 1).ToString();
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult DKTaiKhoan() {

            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DKTaiKhoan(FormCollection collection, HttpPostedFileBase file) {
            NEWSDataContext context = new NEWSDataContext();
            Acount ac = new Acount();
            var id_user = context.Acounts.Max(m => m.ID_User) + 1;      //Tạo ID tăng mới

            var _Username = collection.Get("Username");
            var _Password = Security.MD5(collection.Get("Password"));
            var _Password2 = Security.MD5(collection.Get("Password2"));
            var _Email = collection.Get("Email");
            var _Fullname = collection.Get("Fullname");
            if (_Password == _Password2){
                ac.Username = _Username;
                ac.Password = _Password;
                ac.Email = _Email;
                ac.Fullname = _Fullname;
                ac.ID_User = id_user;
                ac.Permission = 3;                  //gắn quyền user "khách"
                ac.Avatar = "icon user.jpg";        //gắn avarta mặc định
                ac.Active = false;
                ac.Lock = false;
                ac.Introduce_Yourself = "";
                ac.Join = DateTime.Now;
                ac.Likes = 0;
                ac.Cups = 0;
                ac.Dislikes = 0;
                ac.Confirm = false;
                if (file != null && file.ContentLength > 0) //Kiểm tra có upload ảnh chưa
                {
                    string fileName = Path.GetFileName(file.FileName);  
                        if (Path.GetExtension(fileName).ToLower().Contains("jpg") == true || Path.GetExtension(fileName).ToLower().Contains("png") == true || Path.GetExtension(fileName).ToLower().Contains("jpeg") == true || Path.GetExtension(fileName).ToLower().Contains("gif") == true)
                        {
                        // file sẽ lưu vào đương dẫn bên dưới
                        var path = Path.Combine(Server.MapPath("~/Uploads/Avarta"), fileName);
                        file.SaveAs(path);
                        ac.Avatar = file.FileName;
                        }                    
                }
                ViewBag.ThongBao = "Đăng ký tài khoản thành công";
                context.Acounts.InsertOnSubmit(ac);
                context.SubmitChanges();
                return RedirectToAction("LoginHome", "Login");
            }
            else if(_Password != _Password2)
            {
                ViewBag.ThongBao = "Nhập mật khẩu không khớp nhau";
            }
            else
            {
                ViewBag.ThongBao = "Đăng ký tài khoản không thành công";
            }            
            return View();
        }
    }
}
