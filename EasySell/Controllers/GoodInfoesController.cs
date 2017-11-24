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
    public class GoodInfoesController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: GoodInfoes
        public async Task<ActionResult> Index()
        {
            return View(await db.GoodInfoes.ToListAsync());
        }

        // GET: GoodInfoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GoodInfo goodInfo = await db.GoodInfoes.FindAsync(id);
            if (goodInfo == null)
            {
                return HttpNotFound();
            }
            return View(goodInfo);
        }

        // GET: GoodInfoes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GoodInfoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Weight,Brand")] GoodInfo goodInfo)
        {
            if (ModelState.IsValid)
            {
                db.GoodInfoes.Add(goodInfo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(goodInfo);
        }

        // GET: GoodInfoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GoodInfo goodInfo = await db.GoodInfoes.FindAsync(id);
            if (goodInfo == null)
            {
                return HttpNotFound();
            }
            return View(goodInfo);
        }

        // POST: GoodInfoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Weight,Brand")] GoodInfo goodInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(goodInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(goodInfo);
        }

        // GET: GoodInfoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GoodInfo goodInfo = await db.GoodInfoes.FindAsync(id);
            if (goodInfo == null)
            {
                return HttpNotFound();
            }
            return View(goodInfo);
        }

        // POST: GoodInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            GoodInfo goodInfo = await db.GoodInfoes.FindAsync(id);
            db.GoodInfoes.Remove(goodInfo);
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
