using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EasySell.Models;

namespace EasySell.Controllers
{
    public class OrdersController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: Orders
        public async Task<ActionResult> Index(int? status)
        {
            ViewBag.CompletedOrders = db.Orders.Count(d => d.IsActive == 0);
            ViewBag.ProcessingOrders = db.Orders.Count(d => d.IsActive == 1);
            ViewBag.WaitforShippingOrders = db.Orders.Count(d => d.IsShipped == 0);
            ViewBag.UnpaiedOrders = db.Orders.Count(d => d.IsPaid == 0);

            List<OrderViewModel> VMOrders = new List<OrderViewModel>();
            List<Order> Orders = new List<Order>();
            switch (status)
            {
                case 0: // inactive
                    ViewBag.SubTitle = "Completed";
                    Orders = await db.Orders.Where(d => d.IsActive == 0).ToListAsync();
                    break;
                case 1: // active
                    ViewBag.SubTitle = "Processing";
                    Orders = await db.Orders.Where(d => d.IsActive == 1).ToListAsync();
                    break;
                case 2: // wait for ship
                    ViewBag.SubTitle = "Wait for Shipping";
                    Orders = await db.Orders.Where(d => d.IsShipped == 0).ToListAsync();
                    break;
                case 3: // wait for payment
                    ViewBag.SubTitle = "Unpaid";
                    Orders = await db.Orders.Where(d => d.IsPaid == 0).ToListAsync();
                    break;                    
                case 9:
                    ViewBag.SubTitle = "All";
                    Orders = await db.Orders.ToListAsync();
                    break;                    
                default:
                    ViewBag.SubTitle = "All";
                    Orders = await db.Orders.ToListAsync();
                    break;
            }
            foreach (Order order in Orders)
            {
                var orderedgoods = db.OrderedGoods.Where(d => d.OrderID == order.Id);
                double totalprice = 0;
                if(orderedgoods.Any())
                {
                    totalprice = orderedgoods.Sum(d => d.Price);
                }
                else
                {
                    totalprice = 0;
                }
                OrderViewModel orderviewdata = new OrderViewModel
                {
                    OrderInfo = order,
                    CustomerName = db.Customers.Where(d => d.Id == order.customerID).FirstOrDefault().Name,
                    OrderNumber = order.Id.ToString("00000"),
                    OrderedGoodQty = db.OrderedGoods.Count(d => d.OrderID == order.Id),
                    OrderTotalPrice = totalprice
                };
                VMOrders.Add(orderviewdata);
            }
            return View(VMOrders);
        }


        public async Task<ActionResult> Invoice(int? id)
        {
            List<Package> packages = await db.Packages.Where(d => d.OrderID == id).ToListAsync();
            List<OrderedGood> orderedgoods = await db.OrderedGoods.Where(d => d.OrderID == id).ToListAsync();
            Order orderInfo = await db.Orders.FindAsync(id);
            ViewBag.Packages = packages;
            ViewBag.OrderedGoods = orderedgoods;
            ViewBag.OrderInfo = orderInfo;
            ViewBag.OrderID = id;
            ViewBag.SelectedGoodsFromStorage = await db.Storages.Where(d => d.OrderID == id).ToListAsync();
            return View();
        }

        public ActionResult Delivery(int? id)
        {
            return RedirectToAction("Index", "Packages", new { OrderID = id });
        }

        // GET: Orders/ProcessOrder/5
        public async Task<ActionResult> ProcessOrder(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Order order = await db.Orders.FindAsync(id);
            List<Package> packages = await db.Packages.Where(d => d.OrderID == id).ToListAsync();
            List<OrderedGood> orderedgoods = await db.OrderedGoods.Where(d => d.OrderID == id).ToListAsync();
            Order orderInfo = await db.Orders.FindAsync(id);
            ViewBag.Packages = packages;
            ViewBag.OrderedGoods = orderedgoods;
            ViewBag.OrderInfo = orderInfo;
            ViewBag.OrderID = id;
            ViewBag.SelectedGoodsFromStorage = await db.Storages.Where(d => d.OrderID == id).ToListAsync();
            if (packages == null || orderedgoods == null)
            {
                return HttpNotFound();
            }
            return View();
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,customerID,StatusID,order_createtime,order_closetime")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,customerID,StatusID,order_createtime,order_closetime")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
