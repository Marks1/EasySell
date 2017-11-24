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
        public ActionResult Create(int? OrderID)
        {
            ViewBag.OrderID = OrderID;
            return View();
        }

        // POST: OrderedGoods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,GoodID,OrderID,Quantity,SellPrice")] OrderedGood orderedGood)
        {
            if (ModelState.IsValid)
            {
                db.OrderedGoods.Add(orderedGood);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { OrderID = orderedGood.OrderID});
            }

            return View(orderedGood);
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
            return View(orderedGood);
        }

        // POST: OrderedGoods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderedGood orderedGood = await db.OrderedGoods.FindAsync(id);
            db.OrderedGoods.Remove(orderedGood);
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
