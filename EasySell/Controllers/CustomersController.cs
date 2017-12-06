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
using System.IO;

namespace EasySell.Controllers
{
    public class CustomersController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: Customers
        public async Task<ActionResult> Index()
        {
            int CurrentUserID = new SessionManager().CurrentUser.Id;

            List<CustomerViewModel> VMCustomers = new List<CustomerViewModel>();
            List<Customer> Customers = await db.Customers.Where(d=>d.UserID==CurrentUserID).ToListAsync();
            foreach (Customer customer in Customers)
            {
                int ordercount = db.Orders.Count(d => d.customerID == customer.Id);
                int rating = 0;
                if (ordercount <= 3) rating = 1;
                if (ordercount > 3 && ordercount <= 10) rating = 2;
                if (ordercount > 10 && ordercount <= 20) rating = 3;
                if (ordercount > 20 && ordercount <= 40) rating = 4;
                if (ordercount > 40) rating = 5;
                CustomerViewModel vm = new CustomerViewModel
                {
                    CustomerInfo = customer,
                    Rating = rating,
                };
                VMCustomers.Add(vm);
            }
            return View(VMCustomers);
        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            List<OrderViewModel> VMOrders = new List<OrderViewModel>();
            foreach (Order order in db.Orders.Where(d=>d.customerID == id))
            {
                var orderedgoods = db.OrderedGoods.Where(d => d.OrderID == order.Id);
                double totalprice = 0;
                if (orderedgoods.Any())
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
                    CustomerInfo = customer,
                    OrderNumber = order.Id.ToString("00000"),
                    OrderedGoodQty = 0,
                    OrderTotalPrice = totalprice
                };
                VMOrders.Add(orderviewdata);
            }

            CustomerDetailsViewModel VMCustomerDetails = new CustomerDetailsViewModel
            {
                CustomerInfo = customer,
                OrdersOfCustomer = VMOrders
            };
            
            return View(VMCustomerDetails);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Address,Phone,EMail")] Customer customer, HttpPostedFileBase picture)
        {
            string filename = string.Format("{0}.png", Guid.NewGuid().ToString("N"));
            string targetFolder = Server.MapPath("~/Static/image/user");
            string targetPath = Path.Combine(targetFolder, filename);
            try
            {
                //save file to disk    
                if(picture != null)
                    picture.SaveAs(targetPath);
                else
                {
                    filename = "user.png";
                }
            }
            catch(Exception e)
            {
                filename = "user.png";
            }

            //db
            if (ModelState.IsValid)
            {
                int CurrentUserID = new SessionManager().CurrentUser.Id;
                customer.UserID = CurrentUserID;
                customer.CreateAt = DateTime.Now;
                customer.Picture = filename;
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Address,Phone")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        
        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            db.Customers.Remove(customer);
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
