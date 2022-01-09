using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
using PagedList;
namespace WebDauTienVS2._0.Controllers
{
    public class DienDanController : Controller
    {
        //
        // GET: /DienDan/
        public ActionResult Index(int ?id)
        {            
            NEWSDataContext context = new NEWSDataContext();
            var _solutions = from a in context.Acounts
                             join q in context.Questions
                             on a.Username equals q.Username
                             where (q.Parent == 0)
                             select new QuestionsContain()          //Lưu các giá trị vào lớp model
                             {
                                 C_Avatar = a.Avatar,
                                 C_Fullname = a.Fullname,
                                 C_Likes = a.Likes.Value,
                                 C_Cups = a.Cups.Value,
                                 C_Dislike = a.Dislikes.Value,
                                 C_ID_Q = q.ID_Q,
                                 C_Title = q.Title,
                                 C_Contents = q.Contents,
                                 C_CreaData = q.CreaData.ToString(),
                                 C_ModiData = q.ModiData.ToString(),
                                 C_Username = q.Username.ToString(),
                                 C_Hide = q.Hide.Value,
                             };
            _solutions = _solutions.OrderByDescending(n => n.C_ID_Q);
            int pagesize = 5;
            int pageindex = id ?? 1;
            return View(_solutions.ToPagedList(pageindex, pagesize));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ViewQuestion(int? id, int? _id) { //id là chỉ lớp con của câu hỏi tương ứng với câu trả lời, còn _id là dùng để phân trang. Nếu trùng sẽ sinh ra lỗi
            NEWSDataContext context = new NEWSDataContext();
            //var _solutions = from q in context.Questions.OrderByDescending(q => q.ID_Q)
            //                 where (q.Parent == id)     //Hiển thị câu trả lời tương ứng
            //                 select q;
            var _solutions = from a in context.Acounts
                             join q in context.Questions
                             on a.Username equals q.Username
                             where (q.Parent == id)
                             select new QuestionsContain()          //Lưu các giá trị vào lớp model
                             {
                                 C_Avatar = a.Avatar,
                                 C_Fullname = a.Fullname,
                                 C_Likes = a.Likes.Value,
                                 C_Cups = a.Cups.Value,
                                 C_Dislike = a.Dislikes.Value,
                                 C_ID_Q = q.ID_Q,
                                 C_Title = q.Title,
                                 C_Contents = q.Contents,
                                 C_CreaData = q.CreaData.ToString(),
                                 C_ModiData = q.ModiData.ToString(),
                                 C_Username = q.Username.ToString(),
                                 C_Hide = q.Hide.Value,
                                 C_Liked = q.Liked.Value,
                                 C_Parent = q.Parent.Value,
                             };
            _solutions = _solutions.OrderByDescending(n => n.C_Liked);       //Sắp xếp câu trả lời nhiều like nhất nằm đầu
            Question qs = context.Questions.Single(n => n.ID_Q == id);
            ViewBag.Title = qs.Title;
            ViewBag.Contents = qs.Contents;
            ViewBag.CreaData = qs.CreaData;
            ViewBag.Avatar = qs.Avatar;
            ViewBag.fullName = qs.fullName;
            ViewBag.ModiData = qs.ModiData;
            int pagesize = 5;       //Hiển thị 5 câu trên mỗi trang
            int pageindex = _id ?? 1;
            return View(_solutions.ToPagedList(pageindex, pagesize));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ViewQuestion(FormCollection collection, int id, int? _id)
        {
            if (Session["Username"].ToString() == "Login")
            {              //Kiểm tra nếu chưa đăng nhập, thì yêu cầu đăng nhập
                return RedirectToAction("LoginHome", "Login");
            }
            NEWSDataContext context = new NEWSDataContext();
            var id_qs = context.Questions.Max(m => m.ID_Q) + 1;

            Question _qs = new Question();
            _qs.ID_Q = id_qs;
            _qs.Username = Session["Username"].ToString();
            _qs.Contents = collection.Get("Reply");
            _qs.CreaData = DateTime.Now;
            _qs.Avatar = Session["Avarta"].ToString();
            _qs.fullName = Session["Fullname"].ToString();
            _qs.Hide = true;
            _qs.Parent = id;
            _qs.Liked = 0;
            _qs.Users_Liked = "";

            context.Questions.InsertOnSubmit(_qs);
            context.SubmitChanges();
            //Tất cả phần dưới giống với phương thức get, vẫn tiếp tục load lại dữ liệu đã hiển thị phía get
            var _solutions = from a in context.Acounts
                             join q in context.Questions
                             on a.Username equals q.Username
                             where (q.Parent == id)
                             select new QuestionsContain()    //Lưu các giá trị 2 bảng vào vùng chứa lớp model
                             {
                                 C_Avatar = a.Avatar,
                                 C_Fullname = a.Fullname,
                                 C_Likes = a.Likes.Value,
                                 C_Cups = a.Cups.Value,
                                 C_Dislike = a.Dislikes.Value,
                                 C_ID_Q = q.ID_Q,
                                 C_Title = q.Title,
                                 C_Contents = q.Contents,
                                 C_CreaData = q.CreaData.ToString(),
                                 C_ModiData = q.ModiData.ToString(),
                                 C_Username = q.Username.ToString(),
                                 C_Hide = q.Hide.Value,
                                 C_Liked = q.Liked.Value,
                             };
            _solutions = _solutions.OrderByDescending(n => n.C_Liked);     //Sắp xếp câu trả lời mới nhất nằm đầu
            Question qs = context.Questions.Single(n => n.ID_Q == id);
            ViewBag.Title = qs.Title;
            ViewBag.Contents = qs.Contents;
            ViewBag.CreaData = qs.CreaData;
            ViewBag.Avatar = qs.Avatar;
            ViewBag.fullName = qs.fullName;
            ViewBag.ModiData = qs.ModiData;
            int pagesize = 5;       //Hiển thị 5 câu trên mỗi trang
            int pageindex = _id ?? 1;
            return View(_solutions.ToPagedList(pageindex, pagesize));
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Sulutions(int id) {
            NEWSDataContext context = new NEWSDataContext();
            var _sl = from s in context.Solutions.OrderByDescending(s => s.ID_S == id)
                          select s;
            ViewBag.Temp = _sl;
            return View();
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AskQuestion()
        {
            if (Session["Username"].ToString() == "Login" || Session["Username"] == null)
                return RedirectToAction("LoginHome", "Login");
            return View();
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AskQuestion(FormCollection collection)
        {
            NEWSDataContext context = new NEWSDataContext();
            Question qs = new Question();
            var id_qs = context.Questions.Max(m => m.ID_Q) + 1;
            qs.ID_Q = id_qs;
            qs.Username = Session["Username"].ToString();
            qs.Contents = collection.Get("Reply");
            qs.CreaData = DateTime.Now;
            qs.Avatar = Session["Avarta"].ToString();
            qs.fullName = Session["Fullname"].ToString();
            qs.Hide = true;
            qs.Title = collection.Get("Title");
            qs.Parent = 0;
            qs.Liked = 0;
            context.Questions.InsertOnSubmit(qs);
            context.SubmitChanges();
            return RedirectToAction("Index", "DienDan");
        }
        //public ActionResult DeleteQuestion(int id_q)
        //{
        //    NEWSDataContext context = new NEWSDataContext();
        //    Question qs = context.Questions.Single(n => n.ID_Q == id_q);
        //    context.Questions.DeleteOnSubmit(qs);
        //    return RedirectToAction("ViewQuestion", "DienDan");
        //}
        public ActionResult UserLike(int Like_id, string User)
        {
            NEWSDataContext context = new NEWSDataContext();
            Question _qs = context.Questions.Single(n => n.ID_Q == Like_id);
            Acount _ac = context.Acounts.Single(m => m.Username == User);
            if (Session["Username"].ToString() == "Login" || Session["Username"] == null)       //Bắt buộc đăng nhập khi sử dụng chức năng like
            {
                return RedirectToAction("LoginHome", "Login");
            }
            
            //if ( Session["Username"].ToString().Contains(_qs.Users_Liked) == false)
            //{
            //    _qs.Users_Liked = _qs.Users_Liked + " " + Session["Username"].ToString();
            //    _qs.Liked += 1;
            //    context.SubmitChanges();
            //}
            //else
            //{
            //    ViewBag.ThongBao = "<script> alert('Bạn đã like câu trả lời này rồi')</ script > ";
            //}
            if (_qs.Users_Liked.Contains(Session["Username"].ToString()) == false)
            {
                _qs.Users_Liked = _qs.Users_Liked + " " + Session["Username"].ToString();
                _qs.Liked += 1;     //Câu trả lời được tăng 1 like
                _ac.Likes += 1;     //Tài khoản được tăng 1 like
                context.SubmitChanges();
            }
            else
            {
                //Xóa chuỗi sestion["Username"] trong _qs.Users_Liked
                //_qs.Liked -= 1;
            }

            return RedirectToAction("ViewQuestion", "DienDan", new { id = _qs.Parent });
        }
        public ActionResult Profile(string User)
        {
            NEWSDataContext context = new NEWSDataContext();
            Acount ac = context.Acounts.Single(n => n.Username == User);
            if (ac.Confirm == true)     //Xác thực tài khoản
            {
                ViewBag.Confirm = "<i class='fas fa-check - circle'></i>";    //In icon tích xanh nằm cạnh tên file
            }
            return View(ac);
        }
    }

}
