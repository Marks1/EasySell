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
            if (Session["CurrentUserID"] == null)
            {
                RedirectToAction("Login", "Home");
            }
            int CurrentUserID = (int)Session["CurrentUserID"];


            List<OrderViewModel> VMOrders = new List<OrderViewModel>();
            List<Order> Orders = new List<Order>();
            switch (status)
            {
                case 0: // inactive
                    ViewBag.SubTitle = "Completed";
                    Orders = await db.Orders.Where(d => d.IsActive == 0 && d.UserID == CurrentUserID).ToListAsync();
                    break;
                case 1: // active
                    ViewBag.SubTitle = "Processing";
                    Orders = await db.Orders.Where(d => d.IsActive == 1 && d.UserID == CurrentUserID).ToListAsync();
                    break;
                case 2: // wait for ship
                    ViewBag.SubTitle = "Wait for Shipping";
                    Orders = await db.Orders.Where(d => d.IsShipped == 0 && d.UserID == CurrentUserID).ToListAsync();
                    break;
                case 3: // wait for payment
                    ViewBag.SubTitle = "Unpaid";
                    Orders = await db.Orders.Where(d => d.IsPaid == 0 && d.UserID == CurrentUserID).ToListAsync();
                    break;                    
                case 9:
                    ViewBag.SubTitle = "All";
                    Orders = await db.Orders.Where(d=>d.UserID == CurrentUserID).ToListAsync();
                    break;                    
                default:
                    ViewBag.SubTitle = "All";
                    Orders = await db.Orders.Where(d => d.UserID == CurrentUserID).ToListAsync();
                    break;
            }
            
            foreach (Order order in Orders)
            {
                OrderViewModel orderVM = await GetOrderViewModelData(order);
                VMOrders.Add(orderVM);
            }

            OrdersViewModel ordersview = new OrdersViewModel
            {
                ActiveOrdersNum = db.Orders.Count(d => d.IsActive == 0 && d.UserID == CurrentUserID),
                InactiveOrdersNum = db.Orders.Count(d => d.IsActive == 1 && d.UserID == CurrentUserID),
                UnshippedOrdersNum = db.Orders.Count(d => d.IsShipped == 0 && d.UserID == CurrentUserID),
                UnpaidOrdersNum = db.Orders.Count(d => d.IsPaid == 0 && d.UserID == CurrentUserID),                
                OrdersList = VMOrders
            };
            return View(ordersview);
        }

        private async Task<OrderViewModel> GetOrderViewModelData(Order order)
        {
            var orderedgoods = db.OrderedGoods.Where(d => d.OrderID == order.Id);
            double totalprice = 0;
            if (orderedgoods.Any())
            {
                totalprice = await orderedgoods.SumAsync(d => d.TotalPrice) ?? 0;
            }
            else
            {
                totalprice = 0;
            }
            Customer cus = await db.Customers.FindAsync(order.customerID);
            OrderViewModel orderviewdata = new OrderViewModel
            {
                OrderInfo = order,
                CustomerInfo = cus,                
                OrderNumber = order.Id.ToString("00000"),
                OrderedGoodQty = await db.OrderedGoods.CountAsync(d => d.OrderID == order.Id),
                OrderTotalPrice = totalprice
            };
            return orderviewdata;
        }

        public async Task<ActionResult> Invoice(int? id)
        {
            //Order Info
            Order order = await db.Orders.FindAsync(id);
            OrderViewModel OrderInfo = await GetOrderViewModelData(order);
            //Packages
            List<PackageViewModel> Packages = new List<PackageViewModel>();
            double TotalPackagePrice = 0;
            double TotalGoodPrice = 0;
            foreach(Package pkg in db.Packages.Where(d => d.OrderID == id))
            {
                Packages.Add(new PackageViewModel
                {
                    PackageInfo = pkg,
                    DeliverName = db.PackageProviders.Find(pkg.ProviderID).ProviderName
                });
                TotalPackagePrice += pkg.MoneySpend;
            }
            //Ordered Goods
            List<OrderedGoodViewModel> OrderedGoods = new List<OrderedGoodViewModel>();
            foreach(OrderedGood good in db.OrderedGoods.Where(d=>d.OrderID == id))
            {
                OrderedGoods.Add(new OrderedGoodViewModel
                {
                    OrderedGoodInfo = good,
                    GoodName = db.GoodInfoes.Find(good.GoodID).Name
                });
                TotalGoodPrice += good.TotalPrice ?? 0;
            }

            InvoiceViewModel Invoice = new InvoiceViewModel
            {
                OrderedGoods = OrderedGoods,
                OrderVM = OrderInfo,
                Packages = Packages,
                TotalPackagePrice = TotalPackagePrice,
                TotalPrice = TotalGoodPrice + TotalPackagePrice
            };
            return View(Invoice);
        }
        

        // GET: Orders/ProcessOrder/5
        public async Task<ActionResult> ProcessOrder(int? id)
        {           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            double TotalCost = 0;
            List<int> orderedgoodsIDs = new List<int>();
            List<Package> packages = await db.Packages.Where(d => d.OrderID == id).ToListAsync();
            List<OrderedGood> orderedgoods = await db.OrderedGoods.Where(d => d.OrderID == id).ToListAsync();
            List<OrderedGoodViewModel> OrderedGoods = new List<OrderedGoodViewModel>();
            List<PackageViewModel> Packages = new List<PackageViewModel>();
            foreach (OrderedGood good in orderedgoods)
            {
                OrderedGoods.Add(new OrderedGoodViewModel
                {
                    OrderedGoodInfo = good,
                    GoodName = db.GoodInfoes.Find(good.GoodID).Name
                });
                orderedgoodsIDs.Add(good.GoodID);
            }
            foreach(Package pkg in packages)
            {
                Packages.Add(new PackageViewModel
                {
                    PackageInfo = pkg,
                    DeliverName = db.PackageProviders.Find(pkg.ProviderID).ProviderName,
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
            OrderViewModel orderviewdata = await GetOrderViewModelData(db.Orders.Find(id));
            ProcessOrderViewModel processOrderview = new ProcessOrderViewModel
            {
                TotalCost = TotalCost,
                Revenue = Convert.ToDouble(0),
                Duration = 1,
                OrderInfoView = orderviewdata,
                Packages = Packages,
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
                if (Session["CurrentUserID"] == null)
                {
                    RedirectToAction("Login", "Home");
                }
                int CurrentUserID = (int)Session["CurrentUserID"];

                Order neworder = new Order
                {
                    UserID = CurrentUserID,
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



        // GET: Storages/avaiable/5&3
        public async Task<ActionResult> MatchGoods(int? id)
        {
            int CurrentUserID = (int)Session["CurrentUserID"];
            OrderedGood orderedGood = await db.OrderedGoods.FindAsync(id);
            OrderedGoodViewModel orderedgood = new OrderedGoodViewModel
            {
                OrderedGoodInfo = orderedGood,
                GoodName = db.GoodInfoes.Find(orderedGood.GoodID).Name
            };
            List<StorageGoodViewModel> AvaiavleGoodsInStorage = new List<StorageGoodViewModel>();
            foreach (Storage storagegood in db.Storages.Where(d => d.OrderID == null && d.GoodID == orderedGood.GoodID && d.UserID == CurrentUserID))
            {
                AvaiavleGoodsInStorage.Add(new StorageGoodViewModel
                {
                    StorageGoodInfo = storagegood,
                    GoodName = db.GoodInfoes.Find(storagegood.GoodID).Name
                });
            }
            MatchGoodViewModel MatchGoodinStorage = new MatchGoodViewModel
            {
                OrderedGood = orderedgood,
                //OrderInfoView = await GetOrderViewModelData(OrderID),
                AvailableGoodsInStorage = AvaiavleGoodsInStorage
            };
            return View(MatchGoodinStorage);
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
            OrderViewModel orderVM = await GetOrderViewModelData(order);
            return View(orderVM);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            //Remove associated orderedgoods
            db.OrderedGoods.RemoveRange(db.OrderedGoods.Where(d => d.OrderID == id));

            //Remove associated packages
            db.Packages.RemoveRange(db.Packages.Where(d => d.OrderID == id));

            //Update storage to put back goods
            foreach(Storage pkgtoreset in db.Storages.Where(d => d.OrderID == id))
            {
                pkgtoreset.OrderID = null;                
            }
            
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
