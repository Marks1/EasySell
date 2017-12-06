using EasySell.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace EasySell.Controllers
{
    public class SessionManager : Controller
    {
        public User CurrentUser
        {
            set
            {
                Session["CurrentUser"] = value;
            }
            get
            {
                User currentuser = (User)Session["CurrentUser"];
                if(currentuser == null)
                {
                    RedirectToAction("Login", "Home");
                }
                return currentuser;
            }
        }
    }
}