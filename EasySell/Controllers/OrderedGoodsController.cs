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
    public class OrderedGoodsController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: OrderedGoods
        public async Task<ActionResult> Index(int? OrderID)
        {
            ViewBag.OrderID = OrderID;
            return View(await db.OrderedGoods.Where(d=>d.OrderID== OrderID).ToListAsync());
        }

        // GET: OrderedGoods/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderedGood orderedGood = await db.OrderedGoods.FindAsync(id);
            if (orderedGood == null)
            {
                return HttpNotFound();
            }
            return View(orderedGood);
        }

        // GET: OrderedGoods/Create
        public ActionResult Create(int OrderID)
        {
            if (Session["CurrentUserID"] == null)
            {
                RedirectToAction("Login", "Home");
            }
            int CurrentUserID = (int)Session["CurrentUserID"];

            List<SelectListItem> goods = new List<SelectListItem>();
            foreach (GoodInfo good in db.GoodInfoes.Where(d=>d.UserID==CurrentUserID))
            {
                goods.Add(new SelectListItem { Text = good.Name, Value = good.Id.ToString() });
            }
            NewOrderedGoodViewModel neworderedgood = new NewOrderedGoodViewModel
            {
                OrderID = OrderID,
                CustomerID = db.Orders.Find(OrderID).customerID,
                AllGoods = goods,
            };
            return View(neworderedgood);
        }

        // POST: OrderedGoods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SelectedGoodID,OrderID,Quantity,Price,CustomerID")] NewOrderedGoodViewModel neworderedGood)
        {
            if (Session["CurrentUserID"] == null)
            {
                RedirectToAction("Login", "Home");
            }
            int CurrentUserID = (int)Session["CurrentUserID"];


            if (ModelState.IsValid)
            {
                OrderedGood orderedGood = new OrderedGood
                {
                    UserID = CurrentUserID,
                    CustomerID = neworderedGood.CustomerID,
                    GoodID = neworderedGood.SelectedGoodID,
                    Quantity = neworderedGood.Quantity,
                    OrderID = neworderedGood.OrderID,
                    Price = neworderedGood.Price,
                    TotalPrice = neworderedGood.Price * neworderedGood.Quantity
                };
                db.OrderedGoods.Add(orderedGood);
                await db.SaveChangesAsync();
                
            }

            return RedirectToAction("ProcessOrder", "Orders", new { id = neworderedGood.OrderID });
        }

        // GET: OrderedGoods/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderedGood orderedGood = await db.OrderedGoods.FindAsync(id);
            if (orderedGood == null)
            {
                return HttpNotFound();
            }
            return View(orderedGood);
        }

        // POST: OrderedGoods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,GoodID,Quantity,OrderID,SellPrice")] OrderedGood orderedGood)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderedGood).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderedGood);
        }

        // GET: OrderedGoods/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderedGood orderedGood = await db.OrderedGoods.FindAsync(id);
            if (orderedGood == null)
            {
                return HttpNotFound();
            }
            OrderedGoodViewModel orderedgoodVM = new OrderedGoodViewModel
            {
                OrderedGoodInfo = orderedGood,
                GoodName = db.GoodInfoes.Find(orderedGood.GoodID).Name
            };
            
            return View(orderedgoodVM);
        }

        // POST: OrderedGoods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderedGood orderedGood = await db.OrderedGoods.FindAsync(id);
            db.OrderedGoods.Remove(orderedGood);
            await db.SaveChangesAsync();
            return RedirectToAction("ProcessOrder", "Orders", new { id = orderedGood.OrderID });
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
