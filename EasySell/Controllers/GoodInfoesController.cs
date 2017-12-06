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
            int CurrentUserID = new SessionManager().CurrentUser.Id;
            //return View(await db.GoodInfoes.ToListAsync());
            List<GoodInfoViewModel> VMGoodInfo = new List<GoodInfoViewModel>();
            var AllGoods = await db.GoodInfoes.Where(d=>d.UserID==CurrentUserID).ToListAsync();                      

            foreach (GoodInfo good in AllGoods)
            {
                double averagecost = 0;
                var storage = db.Storages.Where(d => d.GoodID == good.Id && d.UserID == CurrentUserID);
                if (storage.Any())
                {
                    averagecost = storage.Average(d => d.Cost);
                }
                double averageprice = 0;
                int totalsold = 0;
                var goodordered = db.OrderedGoods.Where(d => d.GoodID == good.Id && d.UserID == CurrentUserID);
                if (goodordered.Any())
                {
                    averageprice = goodordered.Average(d => d.Price);
                    totalsold = goodordered.Sum(d => d.Quantity);
                }
                GoodInfoViewModel vm = new GoodInfoViewModel {
                    GoodInfo = good,
                    AverageCost = averagecost.ToString("F"),
                    AveragePrice = averageprice.ToString("F"),
                    TotalSold = totalsold
                };
                VMGoodInfo.Add(vm);
            }
            return View(VMGoodInfo);
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
                goodInfo.UserID = new SessionManager().CurrentUser.Id;
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
