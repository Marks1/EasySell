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
                    totalprice = orderedgoods.Sum(d => d.TotalPrice) ?? 0;
                }
                else
                {
                    totalprice = 0;
                }
                Customer cus = db.Customers.Find(order.customerID);
                OrderViewModel orderviewdata = new OrderViewModel
                {
                    OrderInfo = order,                    
                    CustomerName = cus.Name,
                    CustomerPic = cus.Picture,
                    OrderNumber = order.Id.ToString("00000"),
                    OrderedGoodQty = db.OrderedGoods.Count(d => d.OrderID == order.Id),
                    OrderTotalPrice = totalprice
                };
                VMOrders.Add(orderviewdata);
            }

            OrdersViewModel ordersview = new OrdersViewModel
            {
                ActiveOrdersNum = db.Orders.Count(d => d.IsActive == 0),
                InactiveOrdersNum = db.Orders.Count(d => d.IsActive == 1),
                UnshippedOrdersNum = db.Orders.Count(d => d.IsShipped == 0),
                UnpaidOrdersNum = db.Orders.Count(d => d.IsPaid == 0),                
                OrdersList = VMOrders
            };
            return View(ordersview);
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
            /*
             *    public class ProcessOrderViewModel
                    {
                        public double TotalCost { set; get; }
                        public double Revenue { set; get; }
                        public int Duration { set; get; }
                        public OrderViewModel OrderInfo { set; get; }
                        public List<StorageGoodViewModel> AvaiavleGoodsInStorage { set; get; }
                        public List<Package> Packages { set; get; }
                        public List<OrderedGoodViewModel> OrderedGoods { set; get; } 
                        public List<StorageGoodViewModel> AssignedGoodInStorage { set; get; }
             */

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            double TotalCost = 0;
            List<int> orderedgoodsIDs = new List<int>();
            List<Package> packages = await db.Packages.Where(d => d.OrderID == id).ToListAsync();
            List<OrderedGood> orderedgoods = await db.OrderedGoods.Where(d => d.OrderID == id).ToListAsync();
            List<OrderedGoodViewModel> OrderedGoods = new List<OrderedGoodViewModel>();
            foreach (OrderedGood good in orderedgoods)
            {
                OrderedGoods.Add(new OrderedGoodViewModel
                {
                    OrderedGoodInfo = good,
                    GoodName = db.GoodInfoes.Find(good.GoodID).Name
                });
                orderedgoodsIDs.Add(good.GoodID);
            }
            List<StorageGoodViewModel> AvaiavleGoodsInStorage = new List<StorageGoodViewModel>();
            foreach (Storage storagegood in db.Storages.Where(d=>d.OrderID == null && orderedgoodsIDs.Contains(d.GoodID)))
            {
                AvaiavleGoodsInStorage.Add(new StorageGoodViewModel
                {
                    StorageGoodInfo = storagegood,
                    GoodName = db.GoodInfoes.Find(storagegood.GoodID).Name
                });
            }

            List<StorageGoodViewModel> AssignedGoodInStorage = new List<StorageGoodViewModel>();
            foreach (Storage storagegood in db.Storages.Where(d => d.OrderID == id))
            {
                AssignedGoodInStorage.Add(new StorageGoodViewModel
                {
                    StorageGoodInfo = storagegood,
                    GoodName = db.GoodInfoes.Find(storagegood.GoodID).Name
                });
                TotalCost += storagegood.TotalCost ?? 0;
            }
            Order orderInfo = await db.Orders.FindAsync(id);
            Customer cus = db.Customers.Find(orderInfo.customerID);
            OrderViewModel orderviewdata = new OrderViewModel
            {
                OrderInfo = orderInfo,
                CustomerName = cus.Name,
                CustomerPic = cus.Picture,
                OrderNumber = orderInfo.Id.ToString("00000"),
                OrderedGoodQty = 0,
                OrderTotalPrice = Convert.ToDouble(0)
            };
            ProcessOrderViewModel processOrderview = new ProcessOrderViewModel
            {
                TotalCost = TotalCost,
                Revenue = Convert.ToDouble(0),
                Duration = 1,
                OrderInfoView = orderviewdata,
                AvaiavleGoodsInStorage = AvaiavleGoodsInStorage,
                Packages = packages,
                OrderedGoods = OrderedGoods,
                AssignedGoodInStorage = AssignedGoodInStorage
            };

            return View(processOrderview);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            List<SelectListItem> customers = new List<SelectListItem>();
            foreach (Customer customer in db.Customers.Where(d => d.IsActive == 1))
            {
                customers.Add(new SelectListItem { Text = customer.Name, Value = customer.Id.ToString() });
            }
            NewOrderViewModel neworder = new NewOrderViewModel
            {
                AllCuseroms = customers
            };
            return View(neworder);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SelectedCustomerID,PriorityHigh")] NewOrderViewModel order)
        {
            if (ModelState.IsValid)
            {
                Order neworder = new Order
                {
                    customerID = order.SelectedCustomerID,
                    Priority = order.PriorityHigh ? 1 : 0,
                    order_createtime = DateTime.Now,
                };
                db.Orders.Add(neworder);
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
