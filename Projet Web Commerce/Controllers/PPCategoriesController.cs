using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    public class PPCategoriesController : Controller
    {
        private readonly AuthDbContext _context;

        public PPCategoriesController(AuthDbContext context)
        {
            _context = context;
        }

        // GET: PPCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.PPCategories.ToListAsync());
        }

        // GET: PPCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ErrorNoFound", "PPCategories");
            }

            var pPCategories = await _context.PPCategories
                .FirstOrDefaultAsync(m => m.NoCategorie == id);
            if (pPCategories == null)
            {
                return RedirectToAction("ErrorNoFound", "PPCategories");
            }

            return View(pPCategories);
        }

        // GET: PPCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PPCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoCategorie,Description,Details")] PPCategories pPCategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pPCategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pPCategories);
        }

        // GET: PPCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ErrorNoFound", "PPCategories");
            }

            var pPCategories = await _context.PPCategories.FindAsync(id);
            if (pPCategories == null)
            {
                return RedirectToAction("ErrorNoFound", "PPCategories");
            }
            return View(pPCategories);
        }

        // POST: PPCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("NoCategorie,Description,Details")] PPCategories pPCategories)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pPCategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PPCategoriesExists(pPCategories.NoCategorie))
                    {
                        return RedirectToAction("ErrorNoFound", "PPCategories");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pPCategories);
        }

        // GET: PPCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("ErrorNoFound", "PPCategories");
            }

            var pPCategories = await _context.PPCategories
                .FirstOrDefaultAsync(m => m.NoCategorie == id);
            if (pPCategories == null)
            {
                return RedirectToAction("ErrorNoFound", "PPCategories");
            }

            return View(pPCategories);
        }

        // POST: PPCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("ErrorNoFound", "PPCategories");
                }

                var pPCategories = await _context.PPCategories.FindAsync(id);
                if (pPCategories != null)
                {
                    _context.PPCategories.Remove(pPCategories);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("Error", "PPCategories");
                
            }
        }

        private bool PPCategoriesExists(int id)
        {
            return _context.PPCategories.Any(e => e.NoCategorie == id);
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult ErrorNoFound()
        {
            return View();
        }

    }
}
