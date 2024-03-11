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
    public class DettaglioOrdinesController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: DettaglioOrdines
        public ActionResult Index()
        {
            var dettaglioOrdine = db.DettaglioOrdine.Include(d => d.Ordine).Include(d => d.Prodotto);
            return View(dettaglioOrdine.ToList());
        }

        // GET: DettaglioOrdines/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DettaglioOrdine dettaglioOrdine = db.DettaglioOrdine.Find(id);
            if (dettaglioOrdine == null)
            {
                return HttpNotFound();
            }
            return View(dettaglioOrdine);
        }

        // GET: DettaglioOrdines/Create
        public ActionResult Create()
        {
            ViewBag.idOrdine = new SelectList(db.Ordine, "idOrdine", "IndirizzoConsegna");
            ViewBag.idProdotto = new SelectList(db.Prodotto, "idProdotto", "Nome");
            return View();
        }

        // POST: DettaglioOrdines/Create
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idDettaglioOrdine,idOrdine,idProdotto,Qta")] DettaglioOrdine dettaglioOrdine)
        {
            if (ModelState.IsValid)
            {
                db.DettaglioOrdine.Add(dettaglioOrdine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idOrdine = new SelectList(db.Ordine, "idOrdine", "IndirizzoConsegna", dettaglioOrdine.idOrdine);
            ViewBag.idProdotto = new SelectList(db.Prodotto, "idProdotto", "Nome", dettaglioOrdine.idProdotto);
            return View(dettaglioOrdine);
        }

        // GET: DettaglioOrdines/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DettaglioOrdine dettaglioOrdine = db.DettaglioOrdine.Find(id);
            if (dettaglioOrdine == null)
            {
                return HttpNotFound();
            }
            ViewBag.idOrdine = new SelectList(db.Ordine, "idOrdine", "IndirizzoConsegna", dettaglioOrdine.idOrdine);
            ViewBag.idProdotto = new SelectList(db.Prodotto, "idProdotto", "Nome", dettaglioOrdine.idProdotto);
            return View(dettaglioOrdine);
        }

        // POST: DettaglioOrdines/Edit/5
        // Per la protezione da attacchi di overposting, abilitare le proprietà a cui eseguire il binding. 
        // Per altri dettagli, vedere https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idDettaglioOrdine,idOrdine,idProdotto,Qta")] DettaglioOrdine dettaglioOrdine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dettaglioOrdine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idOrdine = new SelectList(db.Ordine, "idOrdine", "IndirizzoConsegna", dettaglioOrdine.idOrdine);
            ViewBag.idProdotto = new SelectList(db.Prodotto, "idProdotto", "Nome", dettaglioOrdine.idProdotto);
            return View(dettaglioOrdine);
        }

        // GET: DettaglioOrdines/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DettaglioOrdine dettaglioOrdine = db.DettaglioOrdine.Find(id);
            if (dettaglioOrdine == null)
            {
                return HttpNotFound();
            }
            return View(dettaglioOrdine);
        }

        // POST: DettaglioOrdines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DettaglioOrdine dettaglioOrdine = db.DettaglioOrdine.Find(id);
            db.DettaglioOrdine.Remove(dettaglioOrdine);
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
    }
}
