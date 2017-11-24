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
    public class PackageProvidersController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: PackageProviders
        public async Task<ActionResult> Index()
        {
            return View(await db.PackageProviders.ToListAsync());
        }

        // GET: PackageProviders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageProvider packageProvider = await db.PackageProviders.FindAsync(id);
            if (packageProvider == null)
            {
                return HttpNotFound();
            }
            return View(packageProvider);
        }

        // GET: PackageProviders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PackageProviders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProviderName")] PackageProvider packageProvider)
        {
            if (ModelState.IsValid)
            {
                db.PackageProviders.Add(packageProvider);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(packageProvider);
        }

        // GET: PackageProviders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageProvider packageProvider = await db.PackageProviders.FindAsync(id);
            if (packageProvider == null)
            {
                return HttpNotFound();
            }
            return View(packageProvider);
        }

        // POST: PackageProviders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProviderName")] PackageProvider packageProvider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(packageProvider).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(packageProvider);
        }

        // GET: PackageProviders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PackageProvider packageProvider = await db.PackageProviders.FindAsync(id);
            if (packageProvider == null)
            {
                return HttpNotFound();
            }
            return View(packageProvider);
        }

        // POST: PackageProviders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PackageProvider packageProvider = await db.PackageProviders.FindAsync(id);
            db.PackageProviders.Remove(packageProvider);
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
