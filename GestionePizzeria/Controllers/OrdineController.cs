using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using GestionePizzeria.Models;

namespace GestionePizzeria.Controllers
{
    
    public class OrdineController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        [Authorize(Roles = "Admin")]
        // GET: Ordine
        public ActionResult Index()
        {
            var ordine = db.Ordine.Include(o => o.Utente);
            return View(ordine.ToList());
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        // GET: Ordine/Create
        public ActionResult Create()
        {
            ViewBag.idUtente = new SelectList(db.Utente, "idUtente", "Username");
            return View();
        }

        // POST: Ordine/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordine ordine = db.Ordine.Find(id);
            db.Ordine.Remove(ordine);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin, User")]
        public ActionResult Cart()
        {


            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult CartQta(int Sum , int idProdotto) 
        {
            var prodotto = db.Prodotto.Find(idProdotto);
            Dictionary<Prodotto, int> Carrello = Session["Carrello"] as Dictionary<Prodotto, int>;


            if (Sum == 1) 
            {
                Carrello[prodotto]++;
                System.Diagnostics.Debug.WriteLine("Quantità incrementata per il prodotto: " + prodotto.Nome);
            }
            else 
            {
                

                Carrello[prodotto]--;
                System.Diagnostics.Debug.WriteLine("Quantità decrementata per il prodotto: " + prodotto.Nome);
                if (Carrello[prodotto] == 0)
                {
                Carrello.Remove(prodotto);
                    System.Diagnostics.Debug.WriteLine("prodotto: " + prodotto.Nome + "rimosso");
                }
              
            }
            return RedirectToAction("Cart" , "Ordine");
        }



        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public ActionResult SendOrder(Ordine O)
        {
            Dictionary<Prodotto, int> Carrello = Session["Carrello"] as Dictionary<Prodotto, int>;
            decimal prezzoTotale = 0;
            System.Diagnostics.Debug.WriteLine("Chiamata metodo SendOrder");

            foreach (KeyValuePair<Prodotto, int> item in Carrello) 
            {
                prezzoTotale += (item.Key.Prezzo * item.Value);
            
            }


            var ordine = new Ordine
            {
                idUtente = Convert.ToInt32(Session["idUtente"]),
                DataOridine = DateTime.Now,
                Importo = prezzoTotale,
                IndirizzoConsegna = O.IndirizzoConsegna,
                Note = O.Note,
                Evaso = false,

            };

            foreach (KeyValuePair<Prodotto, int> item in Carrello) 
            {
                ordine.DettaglioOrdine.Add(new DettaglioOrdine 
                {
                    idProdotto = item.Key.idProdotto,
                    Qta = item.Value,
                });
            }

            db.Ordine.Add(ordine);
            db.SaveChanges();
            Dictionary<Prodotto, int> Carrello2 = new Dictionary<Prodotto, int>();
            Session["Carrello"] = Carrello2;


            return RedirectToAction("Index", "Home");
        }



    }
}
