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
    public class StoragesController : Controller
    {
        private EasysellEntities db = new EasysellEntities();

        // GET: Storages
        public ActionResult Index()
        {
            int CurrentUserID = new SessionManager().CurrentUser.Id;

            List<StorageGoodViewModel> StorageList = new List<StorageGoodViewModel>();
            foreach (Storage storage in db.Storages.Where(d => d.OrderID == null && d.UserID == CurrentUserID))
            {
                StorageGoodViewModel StorageVM = new StorageGoodViewModel
                {
                    StorageGoodInfo = storage,
                    GoodName = db.GoodInfoes.Find(storage.GoodID).Name
                };
                StorageList.Add(StorageVM);
            }
            return View(StorageList);
        }

        // GET: Storages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // GET: Storages/Create
        public ActionResult Create()
        {
            List<SelectListItem> goods = new List<SelectListItem>();
            foreach (GoodInfo good in db.GoodInfoes)
            {
                goods.Add(new SelectListItem { Text = good.Name, Value = good.Id.ToString() });
            }
            NewStorageGoodViewModel newStorage = new NewStorageGoodViewModel
            {
                Quantity = 0,
                Cost = 0,
                AllGoods = goods,
            };
            return View(newStorage);
        }

        // POST: Storages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SelectedGoodID,Cost,Quantity")] NewStorageGoodViewModel newstorage)
        {
            int CurrentUserID = new SessionManager().CurrentUser.Id;

            int goodID = newstorage.SelectedGoodID;
            double cost = newstorage.Cost;
            int quantity = newstorage.Quantity;
            Storage existingitem = await db.Storages.Where(d => d.GoodID == goodID && d.Cost == cost && d.UserID == CurrentUserID).FirstOrDefaultAsync();
            //update exising one
            if (existingitem != null)
            {
                existingitem.Quantity = existingitem.Quantity + quantity;
                db.Entry(existingitem).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            else
            {
                //create new
                Storage newitem = new Storage
                {
                    UserID = CurrentUserID,
                    GoodID = goodID,
                    Cost = cost,
                    Quantity = quantity,
                    TotalCost = cost * quantity
                };
                if (ModelState.IsValid)
                {
                    db.Storages.Add(newitem);
                    await db.SaveChangesAsync();
                    
                }
            }
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> Assign([Bind(Include = "SelectedStorageID,SelectedOrderedGoodID")] MatchGoodViewModel AssignedGoodInStorage)
        {
            if (AssignedGoodInStorage == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(AssignedGoodInStorage.SelectedStorageID);            
            int orderedgoodID = AssignedGoodInStorage.SelectedOrderedGoodID;
            OrderedGood orderedgood = await db.OrderedGoods.FindAsync(orderedgoodID);
            int GoodID = orderedgood.GoodID;

            if (storage == null)
            {
                return HttpNotFound();
            }
            int avaiableamount = storage.Quantity;
            int selectedamount = orderedgood.Quantity;
            if (selectedamount > avaiableamount)
            {
                return HttpNotFound();
            }
            int restofgoodamount = avaiableamount - selectedamount;
            if(restofgoodamount == 0)
            {                
                storage.OrderID = orderedgood.OrderID;
                db.Entry(storage).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            else
            {
                storage.OrderID = orderedgood.OrderID;
                storage.Quantity = selectedamount;
                db.Entry(storage).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //add new record for rest
                Storage newstorage = new Storage
                {
                    GoodID = storage.GoodID,
                    Cost = storage.Cost,
                    Quantity = restofgoodamount,
                    TotalCost = storage.Cost * restofgoodamount,
                };
                db.Storages.Add(newstorage);
                await db.SaveChangesAsync();
            }
            return RedirectToAction("MatchGoods", "Orders", new { id = orderedgoodID });
        }



        // GET: Storages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // POST: Storages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,GoodID,OrderID,BugPrice,Quantity")] Storage storage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(storage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(storage);
        }

        // GET: Storages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Storage storage = await db.Storages.FindAsync(id);
            if (storage == null)
            {
                return HttpNotFound();
            }
            return View(storage);
        }

        // POST: Storages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Storage storage = await db.Storages.FindAsync(id);
            db.Storages.Remove(storage);
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
