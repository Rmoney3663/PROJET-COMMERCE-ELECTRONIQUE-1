using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Projet_Web_Commerce.Controllers
{
    public class PPHistoriquePaiementsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PPHistoriquePaiementsController(IWebHostEnvironment webHostEnvironment, AuthDbContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: PPHistoriquePaiements
        [Authorize(Roles = "Gestionnaire")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.PPHistoriquePaiements.ToListAsync());
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> HistoriqueClient()
        {
            var user = await _userManager.GetUserAsync(User);
            var client = _context.PPClients.Where(c => c.IdUtilisateur == user.Id).FirstOrDefault();

            var historique = await _context.PPHistoriquePaiements.Where(h => h.NoClient == client.NoClient).ToListAsync();
            return View(historique);
        }
        
        private bool PPHistoriquePaiementsExists(int id)
        {
            return _context.PPHistoriquePaiements.Any(e => e.NoHistorique == id);
        }

        public IActionResult Download(int id)
        {
            string fileName = $"{id}.pdf";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "data", "pdf", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.ErrorMessage = $"Le fichier demandé '{fileName}' n'a pas été trouvé.";

                return View("FileNotFound");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }

        public IActionResult DownloadG(int id)
        {
            string fileName = $"{id}.pdf";
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "data", "pdf", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                ViewBag.ErrorMessage = $"Le fichier demandé '{fileName}' n'a pas été trouvé.";

                return View("FileNotFoundG");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, "application/pdf", fileName);
        }
    }
}
