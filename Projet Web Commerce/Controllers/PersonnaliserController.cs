using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;

namespace Projet_Web_Commerce.Controllers
{
    public class PersonnaliserController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Utilisateur> _userManager;

        public PersonnaliserController(AuthDbContext context, Microsoft.AspNetCore.Identity.UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PersonnaliserController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int id, string text, string background, string textCard, string backgroundCard, string recherche, 
            string textRecherche, string backgroundButtonDetail, string textButtonDetail, string backgroundButtonAjouter, string textButtonAjouter,
            string backgroundBarre, string textBarre, string backgroundQuantite, string textQuantite, string font)
        {
            string configuration = "";
            configuration += text + ";" + background + ";" + textCard + ";" + backgroundCard + ";" + recherche + ";" + textRecherche + ";" 
                + backgroundButtonDetail + ";" + textButtonDetail + ";" + backgroundButtonAjouter + ";" + textButtonAjouter + ";" + 
                backgroundBarre + ";" + textBarre + ";" + backgroundQuantite + ";" + textQuantite + ";" + font;

            var vendeur = _context.PPVendeurs.Where(v => v.NoVendeur == id).FirstOrDefault();

            vendeur.Configuration = configuration;

            _context.SaveChanges();

            return View();
        }

        // GET: PersonnaliserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PersonnaliserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PersonnaliserController/Create
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

        // GET: PersonnaliserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PersonnaliserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: PersonnaliserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PersonnaliserController/Delete/5
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
