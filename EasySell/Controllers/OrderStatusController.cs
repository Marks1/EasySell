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
    public class OrderStatusController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: OrderStatus
        public async Task<ActionResult> Index()
        {
            return View(await db.OrderStatus.ToListAsync());
        }

        // GET: OrderStatus/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = await db.OrderStatus.FindAsync(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // GET: OrderStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Status")] OrderStatu orderStatu)
        {
            if (ModelState.IsValid)
            {
                db.OrderStatus.Add(orderStatu);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(orderStatu);
        }

        // GET: OrderStatus/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = await db.OrderStatus.FindAsync(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // POST: OrderStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Status")] OrderStatu orderStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderStatu).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(orderStatu);
        }

        // GET: OrderStatus/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = await db.OrderStatus.FindAsync(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // POST: OrderStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderStatu orderStatu = await db.OrderStatus.FindAsync(id);
            db.OrderStatus.Remove(orderStatu);
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
