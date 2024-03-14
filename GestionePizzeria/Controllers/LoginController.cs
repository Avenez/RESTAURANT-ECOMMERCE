using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using GestionePizzeria.Models;

namespace GestionePizzeria.Controllers
{
    public class LoginController : Controller
    {
        private readonly ModelDBContext _dbContext;

        public LoginController()
        {
            _dbContext = new ModelDBContext();
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (TempData["inserimento"] == null)
            {
                TempData["inserimento"] = false;
            }

            bool ins = (bool)TempData["inserimento"];

            if (ins == false)
            {
                TempData["Inserimento"] = false;
            }
            else
            {
                TempData["Inserimento"] = true;
                
            }
            return View();
            
        }

        // Metodo di login che controlla se lo username e la password siano presenti sul db
        // in modo da fare l'autenticazione tramite FormsAuthentication.SetAuthCookie(U.Username, false);
        [HttpPost]
        public ActionResult Login(Utente U)
        {
            try
            {
                var user = _dbContext.Utente.FirstOrDefault(u => u.Username == U.Username && u.Password == U.Password);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, false);
                    Session["idUtente"] = user.idUtente;
                    Dictionary<Prodotto , int> Carrello = new Dictionary<Prodotto , int>();
                    Session["Carrello"] = Carrello;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Inserimento"] = true;
                    Session["Good"] = false;
                    Session["Messaggio"] = "Utente non registrato";
                    return View(U);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Errore Login" + ex.Message);
                // Gestire l'eccezione in modo appropriato
            }

            return View();
        }

        // LogOut con FormsAuthentication.SignOut();
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
