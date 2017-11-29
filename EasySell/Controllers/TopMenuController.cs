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
    public class TopMenuController : Controller
    {
        private EasysellEntities db = new EasysellEntities();
        // GET: TopMenu
        public async Task<ActionResult> Header()
        {
            //User user = await db.Users.FindAsync(Session["UserID"]);
            User user = new User
            {
                Name = "Test",
                Password = "111",
                Picture = "user.png"
            };
            return View("~/Views/Shared/_TopMenu.cshtml", user);
        }
    }
}