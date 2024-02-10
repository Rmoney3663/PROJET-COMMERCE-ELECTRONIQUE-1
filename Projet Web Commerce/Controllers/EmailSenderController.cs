﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Gestionnaire, Vendeur, Client")]
    public class EmailSenderController : Controller
    {
        // GET: EmailSenderController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string sujet, string message, string envoyeur, string selectedDestinataire)
        {
            
            return View();
        }

        // GET: EmailSenderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EmailSenderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmailSenderController/Create
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

        // GET: EmailSenderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmailSenderController/Edit/5
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

        // GET: EmailSenderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmailSenderController/Delete/5
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
