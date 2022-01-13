using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using WebDauTienVS2._0.Controllers;
using WebDauTienVS2._0.Models;
namespace WebDauTienVS2._0
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Application.Add("online", 0);
            Application.Add("visit", 0);
        }
        protected void Session_Start() {
            MenuProvider menuDrop = new MenuProvider();          
            NEWSDataContext context = new NEWSDataContext();
            Statistical boDiem = context.Statisticals.Single(n => n.ID_TF == 1);
            
            boDiem.Traffic += 1;                                //+ 1 lưu lượng truy cập 
            
            Session.Add("Username", "Login");
            Session.Add("Permission", "0");
            Session.Add("Avarta", "icon user.jpg");
            Session.Add("mn", 8);               //Tham số chỉ danh mục tin tức
            Session.Add("mntl", 5);             //Tham số chỉ danh mục tài liệu
            Session.Add("Likes", 0);
            Session["QuanLyTaiLieuAdmin"] = menuDrop.QuanLyMenuTaiLieu();
            Session["menuTaiLieu"] = menuDrop.menuTaiLieu();    //menu tài liệu
            Session["menu"] = menuDrop.menuMainHoziontal();      //menu tin tức
            
            Session["boDiem"] = boDiem.Traffic.ToString();      //Tổng lượt truy cập website

            context.SubmitChanges();
            
        }
    }
}