using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Projet_Web_Commerce.Models;

namespace Projet_Web_Commerce.Areas.Identity.Data;

// Add profile data for application users by adding properties to the Utilisateur class
public class Utilisateur : IdentityUser
{
    [InverseProperty("Utilisateur")]
    public virtual ICollection<PPClients>? PPClients { get; set; }

    [InverseProperty("Utilisateur")]
    public virtual ICollection<PPVendeurs>? PPVendeurs { get; set; }

    [InverseProperty("Utilisateur")]
    public virtual ICollection<PPGestionnaire>? PPGestionnaire { get; set; }
}

