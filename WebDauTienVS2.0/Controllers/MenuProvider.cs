using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDauTienVS2._0.Models;
namespace WebDauTienVS2._0.Controllers
{
    public class MenuProvider : Controller
    {
        // GET: /MenuProvider/

        
        public string menuMainHoziontal(){
             NEWSDataContext context = new NEWSDataContext();
             var menus = from m in context.Menus_NEWs.Where(m => m.Parent == 0) select m;
             if (menus != null)
             {
                 string listMenu = "<ul id='css3menu1' class='topmenu' <li class='switch'><label onclick='' for='css3menu-switcher'></label></li> ";
                 foreach (Menus_NEW m in (menus as IEnumerable<Menus_NEW>))
                 {
                     listMenu = listMenu + "<li class='topmenu' style='height:33px;line-height:33px;'><a href= '#'>" + m.Lable + "</a>";
                     listMenu = listMenu + subMenuMain1Hoziontal(Convert.ToInt64(m.ID_MN_NEW)) + "</li>";               
                 }
                 return listMenu + "</ul>";
             }
             else {
                 return "";
             }
        }
        protected string subMenuMain1Hoziontal(long id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus_NEWs.Where(m => m.Parent == id) select m;

            if (menus != null) {

                string listMenu = "";
                foreach (Menus_NEW m in (menus as IEnumerable<Menus_NEW>)) {
                 //   if (m.UrLink !=  "#")
                        listMenu = listMenu + "<li><a href=http://congtailieudientudthu.somee.com/NEWS/DanhSachNews/1?mn=" + m.ID_MN_NEW + ">" + m.Lable + "</a>";
                  //  else
                    
                        listMenu = listMenu + subMenuMain2Hoziontal(Convert.ToInt64(m.ID_MN_NEW)) + "</li>";
                                            
                }
                return (listMenu.Length == 0) ? listMenu : "<ul>" + listMenu + "</ul>";
            }
            else
            {
                 return "";
            }
        }
        protected string subMenuMain2Hoziontal(long id) { 
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus_NEWs.Where(m => m.Parent == id) select m;
            if (menus != null) {

                string listMenu = "";
                foreach (Menus_NEW m in (menus as IEnumerable<Menus_NEW>)) {
                    listMenu = listMenu + "<li> <a href=http://congtailieudientudthu.somee.com/NEWS/DanhSachNews/1?mn="
                        + m.ID_MN_NEW + ">" + m.Lable + "</a></li>";
                }
                return (listMenu.Length == 0) ? listMenu : "<ul>" + listMenu + "</ul>";
            }
            else
            {
                 return "";
            }       
        }
        public ActionResult Index()
        {
            return View();
        }
        //Danh sách tài liệu.
        public string menuTaiLieu()
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus.Where(m => m.Parent == 0).OrderBy(m => m.OrderKey) select m;
            if (menus != null)
            {
                string listMenu = "<ul id='css3menu1' class='topmenu' <li class='switch'><label onclick='' for='css3menu-switcher'></label></li> ";
                foreach (Menus m in (menus as IEnumerable<Menus>))
                {
                    listMenu = listMenu + "<li class='topmenu' style='height:33px;line-height:33px;'><a href= '#'>" + m.Lable + "</a>";
                    listMenu = listMenu + menuTaiLieu1(Convert.ToInt64(m.ID_MN)) + "</li>";
                }
                return listMenu + "</ul>";
            }
            else
            {
                return "";
            }
        }
        protected string menuTaiLieu1(long id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus.Where(m => m.Parent == id) select m;

            if (menus != null)
            {

                string listMenu = "";
                foreach (Menus m in (menus as IEnumerable<Menus>))
                {
                    //     if (m.UrLink !=  "#")
                    listMenu = listMenu + "<li><a href=http://congtailieudientudthu.somee.com/Home/TaiLieu/1?mntl=" + m.ID_MN + ">" + m.Lable + "</a>";
                    //      else
                    listMenu = listMenu + menuTaiLieu2(Convert.ToInt64(m.ID_MN)) + "</li>";
                }
                return (listMenu.Length == 0) ? listMenu : "<ul>" + listMenu + "</ul>";
            }
            else
            {
                return "";
            }
        }
        protected string menuTaiLieu2(long id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus.Where(m => m.Parent == id) select m;
            if (menus != null)
            {

                string listMenu = "";
                foreach (Menus m in (menus as IEnumerable<Menus>))
                {
                    listMenu = listMenu + "<li> <a href=http://congtailieudientudthu.somee.com/NEWS/DanhSachNews/1?mn="
                        + m.ID_MN + ">" + m.Lable + "</a></li>";
                }
                return (listMenu.Length == 0) ? listMenu : "<ul>" + listMenu + "</ul>";
            }
            else
            {
                return "";
            }
        }



        //Mấy dòng dưới đây để quản lý menu trong "hệ thống quản lý". Nó tương tự như mấy cái trên, chỉ khác chỗ đường dẫn
        public string QuanLyMenuTaiLieu()
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus.Where(m => m.Parent == 0).OrderBy(m => m.OrderKey) select m;
            if (menus != null)
            {
                string listMenuAdmin = "<ul id='css3menu1' class='topmenu' <li class='switch'><label onclick='' for='css3menu-switcher'></label></li> ";
                foreach (Menus m in (menus as IEnumerable<Menus>))
                {
                    listMenuAdmin = listMenuAdmin + "<li class='topmenu' style='height:33px;line-height:33px;'><a href= '#'>" + m.Lable + "</a>";
                    listMenuAdmin = listMenuAdmin + QuanLyMenuTaiLieu1(Convert.ToInt64(m.ID_MN)) + "</li>";
                }
                return listMenuAdmin + "</ul>";
            }
            else
            {
                return "";
            }
        }
        protected string QuanLyMenuTaiLieu1(long id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus.Where(m => m.Parent == id) select m;

            if (menus != null)
            {

                string listMenuAdmin = "";
                foreach (Menus m in (menus as IEnumerable<Menus>))
                {
                    //     if (m.UrLink !=  "#")
                    listMenuAdmin = listMenuAdmin + "<li><a href=http://congtailieudientudthu.somee.com/admin/ViewPageltems/1?mntl=" + m.ID_MN + ">" + m.Lable + "</a>";
                    //      else
                    listMenuAdmin = listMenuAdmin + QuanLyMenuTaiLieu2(Convert.ToInt64(m.ID_MN)) + "</li>";
                }
                return (listMenuAdmin.Length == 0) ? listMenuAdmin : "<ul>" + listMenuAdmin + "</ul>";
            }
            else
            {
                return "";
            }
        }
        protected string QuanLyMenuTaiLieu2(long id)
        {
            NEWSDataContext context = new NEWSDataContext();
            var menus = from m in context.Menus.Where(m => m.Parent == id) select m;
            if (menus != null)
            {

                string listMenuAdmin = "";
                foreach (Menus m in (menus as IEnumerable<Menus>))
                {
                    listMenuAdmin = listMenuAdmin + "<li> <a href=http://congtailieudientudthu.somee.com/admin/ViewPageltems/1?mntl="
                        + m.ID_MN + ">" + m.Lable + "</a></li>";
                }
                return (listMenuAdmin.Length == 0) ? listMenuAdmin : "<ul>" + listMenuAdmin + "</ul>";
            }
            else
            {
                return "";
            }
        }


    }
}
