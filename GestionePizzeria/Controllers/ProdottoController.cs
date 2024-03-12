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

        [Authorize(Roles = "Admin")]
        // POST: Prodotto/Create
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

        // POST: Prodotto/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Prodotto prodotto = db.Prodotto.Find(id);
            db.Prodotto.Remove(prodotto);
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

        //---------Prodotti----------
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult ListaProdotti()
        {
            DateTime now = DateTime.Now;
            DateTime timeToCheck = now.Date + new TimeSpan(18, 0, 0);

            if (now > timeToCheck)
            {
                var listaProdotti = db.Prodotto.Where(p => p.Pranzo == true).ToList();
                ViewBag.ListaProdotti = listaProdotti;
            }
            else
            {
                var listaProdotti = db.Prodotto.Where(p => p.Cena == true).ToList();
                ViewBag.ListaProdotti = listaProdotti;
            }

            return View();
        }

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
