using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionePizzeria.Models;

namespace GestionePizzeria.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class OrdineController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: Ordine
        public ActionResult Index()
        {
            var ordine = db.Ordine.Include(o => o.Utente);
            return View(ordine.ToList());
        }

        // GET: Ordine/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordine ordine = db.Ordine.Find(id);
            if (ordine == null)
            {
                return HttpNotFound();
            }
            return View(ordine);
        }

        // GET: Ordine/Create
        public ActionResult Create()
        {
            ViewBag.idUtente = new SelectList(db.Utente, "idUtente", "Username");
            return View();
        }

        // POST: Ordine/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idOrdine,idUtente,DataOridine,Importo,IndirizzoConsegna,Note,Evaso")] Ordine ordine)
        {
            if (ModelState.IsValid)
            {
                db.Ordine.Add(ordine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idUtente = new SelectList(db.Utente, "idUtente", "Username", ordine.idUtente);
            return View(ordine);
        }

        // GET: Ordine/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordine ordine = db.Ordine.Find(id);
            if (ordine == null)
            {
                return HttpNotFound();
            }
            ViewBag.idUtente = new SelectList(db.Utente, "idUtente", "Username", ordine.idUtente);
            return View(ordine);
        }

        // POST: Ordine/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idOrdine,idUtente,DataOridine,Importo,IndirizzoConsegna,Note,Evaso")] Ordine ordine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idUtente = new SelectList(db.Utente, "idUtente", "Username", ordine.idUtente);
            return View(ordine);
        }

        // GET: Ordine/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ordine ordine = db.Ordine.Find(id);
            if (ordine == null)
            {
                return HttpNotFound();
            }
            return View(ordine);
        }

        // POST: Ordine/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordine ordine = db.Ordine.Find(id);
            db.Ordine.Remove(ordine);
            db.SaveChanges();
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

        //--------ORDINI----------------

        [HttpGet]
        public ActionResult Cart()
        {


            return View();
        }

        [HttpPost]
        public ActionResult Cart(int Sum) 
        {
            return View();
        }



        [HttpPost]
        public ActionResult SendOrder() 
        {
        

        return RedirectToAction("Index", "Home");
        }
    }
}
