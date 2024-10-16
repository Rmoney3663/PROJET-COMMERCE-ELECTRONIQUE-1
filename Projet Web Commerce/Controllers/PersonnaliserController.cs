﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Controllers
{
    [Authorize(Roles = "Vendeur")]
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
            string backgroundBarre, string textBarre, string backgroundQuantite, string textQuantite, string font, IFormFile image, 
            string backgroundButtonEvaluation, string textButtonEvaluation, bool reset)
        {
            var vendeur = _context.PPVendeurs.Where(v => v.NoVendeur == id).FirstOrDefault();
            string configuration = "";
            if (!reset)
            {
                string nomFileImage = vendeur.NoVendeur.ToString();
                var files = Directory.GetFiles("wwwroot/data/images", nomFileImage + ".*");
                if (image != null && image.Length > 0)
                {
                    string extension = Path.GetExtension(image.FileName);

                    if (extension == ".jfif" || extension == ".pjpeg" || extension == ".jpeg" || extension == ".pjp" || extension == ".jpg")
                    {
                        if (files.Length > 0)
                            System.IO.File.Delete("wwwroot/data/images/" + files[0].Split("\\")[1]);

                        nomFileImage += extension;
                        string tempFilePath = Path.Combine("wwwroot/data/images", nomFileImage);
                        using (Stream fileStream = new FileStream(tempFilePath, FileMode.Create))
                        {
                            image.CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        ViewData["erreur"] = "erreur";
                        if (files.Length > 0)
                            nomFileImage = files[0].Split("\\")[1];
                        else
                            nomFileImage = "none";
                    }
                }
                else if (files.Length > 0)
                    nomFileImage = files[0].Split("\\")[1];
                else
                    nomFileImage = "none";

                configuration += text + ";" + background + ";" + textCard + ";" + backgroundCard + ";" + recherche + ";" + textRecherche + ";"
                    + backgroundButtonDetail + ";" + textButtonDetail + ";" + backgroundButtonAjouter + ";" + textButtonAjouter + ";" +
                    backgroundBarre + ";" + textBarre + ";" + backgroundQuantite + ";" + textQuantite + ";" + font + ";" + nomFileImage + ";" +
                    backgroundButtonEvaluation + ";" + textButtonEvaluation;
            }
            else{
                string nomFileImage = vendeur.NoVendeur.ToString();
                var files = Directory.GetFiles("wwwroot/data/images", nomFileImage + ".*");
                if (files.Length > 0)
                    System.IO.File.Delete("wwwroot/data/images/" + files[0].Split("\\")[1]);

                configuration += "#000000;#ECEBF3;#000000;#FFFFFF;#7C6992;#FFFFFF;#7C6992;#F9E547;" +
                    "#808080;#FFFFFF;#FFFFFF;#000000;#FFFFFF;#000000;Arial;none;#808080;#FFFFFF";
            }

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
