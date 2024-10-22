﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
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
        //Metodo per l'index degli ordini con controllo delle session usate per il feed all'utente
        public ActionResult Index()
        {
            if (Session["inserimento"] == null)
            {
                Session["inserimento"] = false;
            }

            bool ins = (bool)Session["inserimento"];

            if (ins == false)
            {
                TempData["Inserimento"] = false;
            }
            else
            {
                TempData["Inserimento"] = true;
                Session["inserimento"] = false;
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "idOrdine,idUtente,DataOridine,Importo,IndirizzoConsegna,Note,Evaso")] Ordine ordine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ordine).State = EntityState.Modified;
                db.SaveChanges();
                Session["Inserimento"] = true;
                Session["Messaggio"] = " Ordine modificato con Successo";
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
        //Metodo per l'eliminazione di un ordine (non usato)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Ordine ordine = db.Ordine.Find(id);
            db.Ordine.Remove(ordine);
            db.SaveChanges();
            Session["Inserimento"] = true;
            Session["Messaggio"] = " Ordine eliminato con Successo";
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

        //Action per l'accesso al carrello utente
        [HttpGet]
        [Authorize(Roles = "User")]
        public ActionResult Cart()
        {


            return View();
        }

        //Action per la gestione dell'aggiunta o eliminazione di una qta di prodotto
        //Prende un parametro numerico in ingresso e l'oggetto prodotto
        //recuperando la lista di prodotti dalla Session["Carrello"] ne aggiorna le qta
        //Se sum = 1 aggiunge una qta / altrimenti ne elimina una
        //Se le qta del prodotto sono pari a zero lo elimina dal carrello
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


        //Action che elimina un prodotto dal carrello indipendentemente dalle qta inserite
        [Authorize(Roles = "User")]
        public ActionResult RemoveFromCart(int idProdotto)
        {
            var prodotto = db.Prodotto.Find(idProdotto);
            Dictionary<Prodotto, int> Carrello = Session["Carrello"] as Dictionary<Prodotto, int>;
            Carrello.Remove(prodotto);
            return RedirectToAction("Cart", "Ordine");
        }


        //Metodo che invia l'ordine
        //1)Recupera il carrello
        //2)Per ogni oggetto * qta calcola il prezzo totale dell'ordine
        //3)Crea un nuovo oggetto ordine
        //4)Per ogni oggetto nel carrello crea un dettaglio ordine che lega all'id dell'ordine creato
        //5)svuota il carrello e invia un feed all'user
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public ActionResult SendOrder(Ordine O)
        {
            if (ModelState.IsValid)
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
                Session["Inserimento"] = true;
                Session["Messaggio"] = " Ordine effettuato con Successo";

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Cart", "Ordine");
        }

        //Action che setta come evaso un ordine
        [Authorize(Roles = "Admin")]
        public ActionResult EvadiOrdine(int id) 
        {
            var OrdineDaModificare = db.Ordine.Find(id);

            if (OrdineDaModificare != null) 
            {
                OrdineDaModificare.Evaso = true;
                db.SaveChanges();
                Session["Inserimento"] = true;
                Session["Messaggio"] = " Ordine evaso con Successo";
            }

            return RedirectToAction("Index", "Ordine");
        }
    }
}
