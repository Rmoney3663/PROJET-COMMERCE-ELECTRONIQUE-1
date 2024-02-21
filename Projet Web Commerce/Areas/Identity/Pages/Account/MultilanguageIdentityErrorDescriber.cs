using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SharpDX.DXGI;

namespace Projet_Web_Commerce.Areas.Identity.Pages.Account
{
    public class MultilanguageIdentityErrorDescriber : IdentityErrorDescriber
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MultilanguageIdentityErrorDescriber(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateEmail),
                Description = string.Format(_localizer["L'adresse de courriel {0} est déjà utilisé."], email)
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {

            return new IdentityError()
            {
                Code = nameof(DuplicateEmail),
                Description = string.Format(_localizer["L'adresse de courriel {0} est déjà utilisé."], userName)
            };
        }
    }
}
