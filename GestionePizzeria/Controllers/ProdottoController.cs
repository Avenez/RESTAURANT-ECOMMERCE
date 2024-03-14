using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestionePizzeria.Models;

namespace GestionePizzeria.Controllers
{
    
    public class ProdottoController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        [Authorize(Roles = "Admin")]
        // GET: Prodotto
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

            return View(db.Prodotto.ToList());
        }

        [Authorize(Roles = "Admin")]
        // GET: Prodotto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            return View(prodotto);
        }

        [Authorize(Roles = "Admin")]
        // GET: Prodotto/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }


        //Action per la creazione di un prodotto con l'aggiunta della possibilità di caricare immagini
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Prodotto prodotto, HttpPostedFileBase Foto)
        {
            if (Foto != null && Foto.ContentLength > 0)
            {
                string nomeFile = Path.GetFileName(Foto.FileName);
                string pathToSave = Path.Combine(Server.MapPath("~/ImmaginiProdotto"), nomeFile);
                Foto.SaveAs(pathToSave);

                prodotto.Foto = "/ImmaginiProdotto/" + nomeFile; // Imposta l'URL dell'immagine
            }

            if (ModelState.IsValid)
            {
                db.Prodotto.Add(prodotto);
                db.SaveChanges();
                Session["Inserimento"] = true;
                Session["Messaggio"] = " Prodotto inserito con Successo";
                return RedirectToAction("Index");
            }

            return View(prodotto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        // GET: Prodotto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            TempData["fotoNome"] = prodotto.Foto;
            return View(prodotto);
        }

        // POST: Prodotto/Edit/5
        //action per la modifica di un prodotto con l'aggiunta della possibilità di caricare immagini
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Prodotto prodotto, HttpPostedFileBase Foto)
        {

            if (Foto != null && Foto.ContentLength > 0)
            {
                string nomeFile = Path.GetFileName(Foto.FileName);
                string pathToSave = Path.Combine(Server.MapPath("~/ImmaginiProdotto"), nomeFile);
                Foto.SaveAs(pathToSave);

                prodotto.Foto = "/ImmaginiProdotto/" + nomeFile; // Imposta l'URL dell'immagine
            }
            else
            {
                prodotto.Foto = TempData["fotoNome"].ToString();
            }


            if (ModelState.IsValid)
            {
                db.Entry(prodotto).State = EntityState.Modified;
                db.SaveChanges();
                Session["Inserimento"] = true;
                Session["Messaggio"] = " Prodotto modificato con Successo";
                return RedirectToAction("Index", "Prodotto");
            }

            return View(prodotto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        // GET: Prodotto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prodotto prodotto = db.Prodotto.Find(id);
            if (prodotto == null)
            {
                return HttpNotFound();
            }
            return View(prodotto);
        }

        //action per l'eliminazione di un prodotto con l'aggiunta dell'eliminazione di tutti i dettagli prodotto relativi a quel prodotto (Una forzatura per sperimentare sulle FK)
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Prodotto prodotto = db.Prodotto.Find(id);
            var dettaglioOrdiniToDelete = db.DettaglioOrdine.Where(d => d.idProdotto == id);
            db.DettaglioOrdine.RemoveRange(dettaglioOrdiniToDelete);
            db.Prodotto.Remove(prodotto);
            db.SaveChanges();
            Session["Inserimento"] = true;
            Session["Messaggio"] = " Prodotto eliminato con Successo";
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

        //---------Prodotti----------
        //Action che restituisce la lista dei prodotti all'user in base all'orario della giornata 
        //prima delle 18:00 è disponibile solo il menù pranzo dopo le 18:00 solo quello cena
        //Questo va in base al marker posto sui prodotti
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult ListaProdotti()
        {
            DateTime now = DateTime.Now;
            DateTime timeToCheck = now.Date + new TimeSpan(18, 0, 0);

            if (now > timeToCheck)
            {
                var listaProdotti = db.Prodotto.Where(p => p.Cena == true).ToList();
                ViewBag.ListaProdotti = listaProdotti;
                ViewBag.MenuType = "Cena";
            }
            else
            {
                var listaProdotti = db.Prodotto.Where(p => p.Pranzo == true).ToList();
                ViewBag.ListaProdotti = listaProdotti;
                ViewBag.MenuType = "Pranzo";
            }

            return View();
        }

        //Action che  aggiunge un prodotto al carrello
        //1) controlla se "Carrello" non sia null e nel caso la riempie
        //2)controlla se il carrello già contiene il prodotto selezionato. Se "false" lo aggiunge, se "true" ne aggiunge una qta
         [Authorize(Roles = "Admin, User")]
        public ActionResult AddToCart(int idProdotto)
        {
            System.Diagnostics.Debug.WriteLine("Id Prodotto: " + idProdotto);

            var prodotto = db.Prodotto.Find(idProdotto);
            Dictionary<Prodotto, int> Carrello = Session["Carrello"] as Dictionary<Prodotto, int>;

            if (Carrello == null)
            {
                Carrello = new Dictionary<Prodotto, int>();
                System.Diagnostics.Debug.WriteLine("Creo Dictonary");
            }

            if (Carrello.ContainsKey(prodotto))
            {
                
                // Il prodotto è già presente nel carrello, quindi incrementa la quantità
                Carrello[prodotto]++;
                System.Diagnostics.Debug.WriteLine("Quantità incrementata per il prodotto: " + prodotto.Nome);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(prodotto);
                // Il prodotto non è presente nel carrello, quindi aggiungilo con una quantità di 1
                Carrello.Add(prodotto, 1);
                System.Diagnostics.Debug.WriteLine("Prodotto aggiunto al carrello: " + prodotto.Nome);
            }

            Session["Carrello"] = Carrello;

            return RedirectToAction("ListaProdotti", "Prodotto");
        }



    }
}
