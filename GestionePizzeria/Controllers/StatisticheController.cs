using GestionePizzeria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestionePizzeria.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StatisticheController : Controller
    {

        public ModelDBContext db = new ModelDBContext();
        // GET: Statistiche
        public ActionResult Statistics()
        {
            return View();
        }


        public JsonResult TotEvasi() 
        {
            DateTime dataInizio = DateTime.Today;
            DateTime dataFine = DateTime.Today.AddDays(1).AddSeconds(-1);



            var TotOrdiniEvasi = db.Ordine.Where(o => o.Evaso == true).Where(o => o.DataOridine > dataInizio).Where(o => o.DataOridine < dataFine).Count();

            return Json(TotOrdiniEvasi, JsonRequestBehavior.AllowGet);
        }


        public JsonResult TotIncasso()
        {

            DateTime dataInizio = DateTime.Today;
            DateTime dataFine = DateTime.Today.AddDays(1).AddSeconds(-1);

            var TotIncassoOrdini = db.Ordine.Where(o => o.Evaso == true && o.DataOridine > dataInizio && o.DataOridine < dataFine).Sum(o => (decimal?)o.Importo) ?? 0;

            return Json(TotIncassoOrdini, JsonRequestBehavior.AllowGet);
        }
    }
}