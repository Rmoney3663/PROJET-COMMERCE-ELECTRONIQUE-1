using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    public class EvaluationsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public EvaluationsController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EvaluationController
        public ActionResult Index(int id)
        {
            ViewData["idProduit"] = id;

            return View();
        }

        [HttpPost]
        public ActionResult Index(int id, int rating)
        {
            ViewData["idProduit"] = id;
            ViewData["rating"] = rating + 1;
            return View();
        }

        // GET: EvaluationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EvaluationController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EvaluationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: EvaluationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, string msg, int noClient, int rating, bool eval)
        {
            DateTime dateCreate = DateTime.Now;
            DateTime dateMaj = DateTime.Now;
            PPEvaluations newEvaluation = new PPEvaluations();

            if (eval)
            {
                var evalActu = _context.PPEvaluations.Where(e => e.NoClient == noClient && e.NoProduit == id).FirstOrDefault();
                evalActu.Commentaire = msg;
                evalActu.Cote = rating;
                evalActu.DateMAJ = dateMaj;
                _context.SaveChanges();
            }
            else
            {
                newEvaluation.NoProduit = id;
                newEvaluation.NoClient = noClient;
                newEvaluation.Commentaire = msg;
                newEvaluation.Cote = rating;
                newEvaluation.DateCreation = dateCreate;
                newEvaluation.DateMAJ = dateMaj;

                _context.Add(newEvaluation);
                _context.SaveChanges();
            }

            ViewData["idProduit"] = id;

            return RedirectToAction(nameof(Index), new RouteValueDictionary(new { controller = "Evaluations", action = "Index", id = id }));
        }

        // GET: EvaluationController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EvaluationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
