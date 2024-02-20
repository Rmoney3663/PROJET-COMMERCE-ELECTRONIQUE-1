// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<Utilisateur> _userManager;
        public readonly AuthDbContext _context;

        public ConfirmEmailModel(AuthDbContext context, UserManager<Utilisateur> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "Merci d'avoir confirmé votre courriel." : "Une erreur est survenue lors de la confirmation de votre courriel.";
            if (result.Succeeded)
            {
                var client = _context.PPClients.FirstOrDefault(c => c.IdUtilisateur == user.Id);
                if (client != null)
                {
                    client.Statut = 1; 
                    await _context.SaveChangesAsync();
                }
            }
            return Page();
        }
    }
}
