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
    public class PackagesController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: Packages
        public async Task<ActionResult> Index(int? OrderID)
        {
            ViewBag.OrderID = OrderID;
            return View(await db.Packages.Where(d=>d.OrderID==OrderID).ToListAsync());
        }

        // GET: Packages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // GET: Packages/Create
        public async Task<ActionResult> Create(int? OrderID)
        {
            if (OrderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.OrderID = OrderID;
            List<PackageProvider> delivers = await db.PackageProviders.Where(d => d.IsActive == 1).ToListAsync();
            NewPackageViewModel packageproviderVM = new NewPackageViewModel
            {
                AllPackageDelivers = delivers,
                SerialNumber = "",
                OrderID = OrderID ?? 0,
            };
            return View(packageproviderVM);
        }

        // POST: Packages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SelectedDeliverID,SerialNumber,MoneySpend,MoneyReceived,OrderID")] NewPackageViewModel package)
        {
            if (ModelState.IsValid)
            {
                int CurrentUserID = new SessionManager().CurrentUser.Id;
                Package newPackage = new Package
                {
                    SerialNumber = package.SerialNumber,
                    ProviderID = package.SelectedDeliverID,
                    OrderID = package.OrderID,
                    UserID = CurrentUserID
                };

                db.Packages.Add(newPackage);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("ProcessOrder", "Orders", new { id = package.OrderID });
        }

        // GET: Packages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,SerialNumber,ProviderID,MoneySpend,MoneyReceived,OrderID")] Package package)
        {
            if (ModelState.IsValid)
            {
                db.Entry(package).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(package);
        }

        // GET: Packages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Package package = await db.Packages.FindAsync(id);
            if (package == null)
            {
                return HttpNotFound();
            }
            return View(package);
        }

        // POST: Packages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Package package = await db.Packages.FindAsync(id);
            db.Packages.Remove(package);
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
