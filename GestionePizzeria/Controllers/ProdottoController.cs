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
    [Authorize(Roles = "Admin")]
    public class ProdottoController : Controller
    {
        private ModelDBContext db = new ModelDBContext();

        // GET: Prodotto
        public ActionResult Index()
        {
            return View(db.Prodotto.ToList());
        }

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

        // GET: Prodotto/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Prodotto prodotto = db.Prodotto.Find(id);
            db.Prodotto.Remove(prodotto);
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


        public ActionResult AddToCart(int idProdotto) 
        {
            var prodotto = db.Prodotto.Find(idProdotto);
            Dictionary<Prodotto, int> Carrello = Session["Carrello"] as Dictionary<Prodotto, int>;

            if (Carrello != null && Carrello.ContainsKey(prodotto))
            {
                Carrello[prodotto]++;
            }
            else 
            {
                Carrello.Add(prodotto, 1);
            }

            Session["Carrello"] = Carrello;

            return RedirectToAction("ListaProdotti", "Prodotto");
        }
    }
}
