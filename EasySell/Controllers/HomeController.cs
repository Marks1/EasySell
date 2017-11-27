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
    public class HomeController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        public ActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            DateTime statofthisweek = DateTime.Now.AddDays(-7);
            DateTime startoflastweek = DateTime.Now.AddDays(-14);
            // order number
            List<Order> _ordersofthisweek = db.Orders.Where(d => d.order_createtime > statofthisweek).ToList();
            List<Order> _ordersoflastweek = db.Orders.Where(d => d.order_createtime > startoflastweek && d.order_createtime < statofthisweek).ToList();
            int thisweek_ordercount = _ordersofthisweek.Count;
            int lastweek_ordercount = _ordersoflastweek.Count;
            ViewBag.OrdersOfThisWeek = thisweek_ordercount;
            ViewBag.OrderIncrease = (thisweek_ordercount - lastweek_ordercount) * 100.0 / (lastweek_ordercount);

            // this week status
            List<Order> _completedordersofthisweek = db.Orders.Where(d => d.order_createtime > statofthisweek && d.IsActive == 0).ToList();
            List<Order> _completedordersoflastweek = db.Orders.Where(d => d.order_createtime > startoflastweek && d.order_createtime < statofthisweek && d.IsActive == 0).ToList();
            double thisweek_totalsell = 0;
            foreach (var thisweekorder in _completedordersofthisweek)
            {
                thisweek_totalsell += db.OrderedGoods.Where(d => d.OrderID == thisweekorder.Id).Sum(d=>d.Price);
            }
            double lastweek_totalsell = 0;
            foreach (var lastweekorder in _completedordersoflastweek)
            {
                lastweek_totalsell += db.OrderedGoods.Where(d => d.OrderID == lastweekorder.Id).Sum(d => d.Price);
            }            
            ViewBag.SalesOfThisWeek = thisweek_totalsell;
            ViewBag.SalesIncrease = (thisweek_totalsell - lastweek_totalsell) * 100.0 / (lastweek_totalsell);

            double thisweek_cost = 0;
            foreach (var thisweekorder in _completedordersofthisweek)
            {
                thisweek_cost += db.Storages.Where(d => d.OrderID == thisweekorder.Id).Sum(d => d.Cost);
            }
            double lastweek_cost = 0;
            foreach (var lastweekorder in _completedordersoflastweek)
            {
                lastweek_cost += db.Storages.Where(d => d.OrderID == lastweekorder.Id).Sum(d => d.Cost);
            }
            ViewBag.CostOfThisWeek = thisweek_totalsell;
            ViewBag.CostIncrease = (thisweek_totalsell - lastweek_totalsell) * 100.0 / (lastweek_totalsell);

            // total
            List<Order> _completedorders = db.Orders.Where(d => d.IsActive == 0).ToList();
            double totalsales = 0;
            double totalcost = 0;
            
            foreach (var order in _completedorders)
            {
                totalsales += db.OrderedGoods.Where(d => d.OrderID == order.Id).Sum(d => d.Price);
            }
            foreach (var order in _completedorders)
            {
                totalcost += db.Storages.Where(d => d.OrderID == order.Id).Sum(d => d.Cost);
            }
            double totalrevenue = totalsales - totalcost;
            ViewBag.TotalSales = totalsales;
            ViewBag.TotalCost = totalcost;
            ViewBag.TotalRevenue = totalrevenue;
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}