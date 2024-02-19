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

namespace Projet_Web_Commerce.Controllers
{
    public class PPHistoriquePaiementsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<Utilisateur> _userManager;

        public PPHistoriquePaiementsController(AuthDbContext context, UserManager<Utilisateur> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        // GET: PPHistoriquePaiements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPHistoriquePaiements = await _context.PPHistoriquePaiements
                .FirstOrDefaultAsync(m => m.NoHistorique == id);
            if (pPHistoriquePaiements == null)
            {
                return NotFound();
            }

            return View(pPHistoriquePaiements);
        }

        // GET: PPHistoriquePaiements/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: PPHistoriquePaiements/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("NoHistorique,MontantVenteAvantLivraison,NoVendeur,NoClient,NoCommande,DateVente,NoAutorisation,FraisLesi,Redevance,FraisLivraison,FraisTPS,FraisTVQ")] PPHistoriquePaiements pPHistoriquePaiements)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(pPHistoriquePaiements);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(pPHistoriquePaiements);
        //}

        //// GET: PPHistoriquePaiements/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var pPHistoriquePaiements = await _context.PPHistoriquePaiements.FindAsync(id);
        //    if (pPHistoriquePaiements == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(pPHistoriquePaiements);
        //}

        // POST: PPHistoriquePaiements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("NoHistorique,MontantVenteAvantLivraison,NoVendeur,NoClient,NoCommande,DateVente,NoAutorisation,FraisLesi,Redevance,FraisLivraison,FraisTPS,FraisTVQ")] PPHistoriquePaiements pPHistoriquePaiements)
        //{
        //    if (id != pPHistoriquePaiements.NoHistorique)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(pPHistoriquePaiements);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PPHistoriquePaiementsExists(pPHistoriquePaiements.NoHistorique))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(pPHistoriquePaiements);
        //}

        // GET: PPHistoriquePaiements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pPHistoriquePaiements = await _context.PPHistoriquePaiements
                .FirstOrDefaultAsync(m => m.NoHistorique == id);
            if (pPHistoriquePaiements == null)
            {
                return NotFound();
            }

            return View(pPHistoriquePaiements);
        }

        // POST: PPHistoriquePaiements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pPHistoriquePaiements = await _context.PPHistoriquePaiements.FindAsync(id);
            if (pPHistoriquePaiements != null)
            {
                _context.PPHistoriquePaiements.Remove(pPHistoriquePaiements);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PPHistoriquePaiementsExists(int id)
        {
            return _context.PPHistoriquePaiements.Any(e => e.NoHistorique == id);
        }
    }
}
