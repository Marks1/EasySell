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

        public async Task<ActionResult> Dashboard()
        {
            DateTime statofthisweek = DateTime.Now.AddDays(-7);
            DateTime startoflastweek = DateTime.Now.AddDays(-14);
            // orders             
            List<int> _ordersofthisweek = await db.Orders.Where(d => d.order_createtime > statofthisweek).Select(s=>s.Id).ToListAsync();
            List<int> _ordersoflastweek = await db.Orders.Where(d => d.order_createtime > startoflastweek && d.order_createtime < statofthisweek).Select(s => s.Id).ToListAsync();

            int thisweek_ordercount = _ordersofthisweek.Count;
            int lastweek_ordercount = _ordersoflastweek.Count;
            double ordercountIncrease = lastweek_ordercount==0? 0: (thisweek_ordercount - lastweek_ordercount) * 100.0 / (lastweek_ordercount);


            double thisweek_totalsell = db.OrderedGoods.Where(d => _ordersofthisweek.Contains(d.OrderID)).Sum(d => (double?)d.TotalPrice) ?? Convert.ToDouble(0);
            double thisweek_cost = db.Storages.Where(d => _ordersofthisweek.Contains(d.OrderID.Value)).Sum(d => (double?)d.TotalCost) ?? Convert.ToDouble(0);            
            double lastweek_totalsell = db.OrderedGoods.Where(d => _ordersoflastweek.Contains(d.OrderID)).Sum(d => (double?)d.TotalPrice) ?? Convert.ToDouble(0);
            double lastweek_cost = db.Storages.Where(d => _ordersoflastweek.Contains(d.OrderID.Value)).Sum(d => (double?)d.TotalCost) ?? Convert.ToDouble(0);
            double salesincrease = lastweek_totalsell==0? 0:(thisweek_totalsell - lastweek_totalsell) * 100.0 / (lastweek_totalsell);
            double costincrease = lastweek_cost == 0 ? 0 : (thisweek_cost - lastweek_cost) * 100.0 / (lastweek_cost);
            // total
            List<int> _allorders = await db.Orders.Select(s => s.Id).ToListAsync();
            double totalsales = db.OrderedGoods.Where(d => _allorders.Contains(d.OrderID)).Sum(d => (double?)d.TotalPrice) ?? Convert.ToDouble(0);
            double totalcost = db.Storages.Where(d => _allorders.Contains(d.OrderID.Value)).Sum(d => (double?)d.TotalCost) ?? Convert.ToDouble(0);
           
            // Top customers
            var topcustomers = await db.OrderedGoods.GroupBy(g => g.CustomerID).Select(x => new
            {
                CustomerID = x.Key,
                OrderCount = x.Count(),
                TotalOrdersAmount = x.Sum(s => s.TotalPrice)
            }).OrderByDescending(o=>o.TotalOrdersAmount).Take(4).ToListAsync();
            List<TopCustomerViewModel> TopCustomers = new List<TopCustomerViewModel>();
            foreach(var topcustomer in topcustomers)
            {
                string customername = db.Customers.Find(topcustomer.CustomerID).Name;
                TopCustomers.Add(new TopCustomerViewModel
                {
                    CustomerID = topcustomer.CustomerID.Value,
                    CustomerName = customername,
                    TotalOrderedAmount = topcustomer.TotalOrdersAmount ?? 0,
                    TotalOrders = topcustomer.OrderCount
                });
            }

            //Top good sold count
            // Top customers
            var topgoodsbycount = await db.OrderedGoods.GroupBy(g => g.GoodID).Select(x => new
            {
                GoodID = x.Key,
                TotalGoodsAmount = x.Sum(s => s.Quantity)
            }).OrderByDescending(o => o.TotalGoodsAmount).Take(5).ToListAsync();
            List<TopGoodCountViewModel> TopGoodsByCount = new List<TopGoodCountViewModel>();
            foreach (var topgood in topgoodsbycount)
            {
                string goodName = db.GoodInfoes.Find(topgood.GoodID).Name;
                TopGoodsByCount.Add(new TopGoodCountViewModel
                {
                    GoodID = topgood.GoodID,
                    GoodName = goodName,
                    SoldCount = topgood.TotalGoodsAmount
                });
            }

            DashboardViewModel dashboardVM = new DashboardViewModel
            {
                OrdersOfThisWeek = thisweek_ordercount,
                OrderIncrease = ordercountIncrease,
                SalesOfThisWeek = thisweek_totalsell,
                SalesIncrease = salesincrease,
                CostOfThisWeek = thisweek_cost,
                CostIncrease = costincrease,
                TotalSales = totalsales,
                TotalCost = totalcost,
                TotalRevenue = totalsales - totalcost,
                TopCustomers = TopCustomers,
                TopGoodsByCount = TopGoodsByCount,
            };

            return View(dashboardVM);
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