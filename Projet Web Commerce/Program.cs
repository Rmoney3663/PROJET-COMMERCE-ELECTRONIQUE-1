using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
using Projet_Web_Commerce.Migrations;
using Projet_Web_Commerce.Models;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<Utilisateur>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>().AddEntityFrameworkStores<AuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MainMenu}/{action=Catalogue}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Utilisateur>>();
    var rolemanagerIdentity = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //var rolemanagerCustom = scope.ServiceProvider.GetRequiredService<RoleManager<TypesUtilisateur>>();

    var roles = new[] { "Gestionnaire", "Client", "Vendeur" };
    var types = new[] { "G", "C", "V" };


    for (var i = 0; i < roles.Length; i++)
    {
        if (!await rolemanagerIdentity.RoleExistsAsync(roles[i]))
        {
            var newRole = new IdentityRole(roles[i]);
            await rolemanagerIdentity.CreateAsync(newRole);
        }
    }

    // Specify the email you want to check
    var emailToCheck = "william.anthony.burgess@gmail.com";
    var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
    optionsBuilder.UseSqlServer("Data Source=tcp:424sql.cgodin.qc.ca,5433;Initial Catalog=BDB68_424Q24;User ID=B68equipe424q24;Password=Password24;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Integrated Security=False");

    var context = new AuthDbContext(optionsBuilder.Options);
    // Check if a user with the specified email already exists
    var existingUser = await userManager.FindByEmailAsync(emailToCheck);
    if (existingUser == null)
    {
        // Create a user
        var user = new Utilisateur
        {
            UserName = emailToCheck,
            Email = emailToCheck,
            EmailConfirmed = true,

        };

        var result = await userManager.CreateAsync(user, "Password1!");


        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Gestionnaire");




            PPGestionnaire newRecord = new PPGestionnaire()
            { IdUtilisateur = user.Id, NoGestionnaire = 100, MotDePasse = "Password1!", DateCreation = DateTime.Now, AdresseEmail = "william.anthony.burgess@gmail.com",  };

            context.PPGestionnaire.Add(newRecord);
            context.SaveChanges();

            List<Province> provinces = new List<Province>
            {
                new Province { ProvinceID = "AB", Nom = "Alberta" },
                new Province { ProvinceID = "BC", Nom = "Colombie-Britannique" },
                new Province { ProvinceID = "MB", Nom = "Manitoba" },
                new Province { ProvinceID = "NB", Nom = "Nouveau-Brunswick" },
                new Province { ProvinceID = "NL", Nom = "Terre-Neuve-et-Labrador" },
                new Province { ProvinceID = "NS", Nom = "Nouvelle-�cosse" },
                new Province { ProvinceID = "NT", Nom = "Territoires du Nord-Ouest" },
                new Province { ProvinceID = "NU", Nom = "Nunavut" },
                new Province { ProvinceID = "ON", Nom = "Ontario" },
                new Province { ProvinceID = "PE", Nom = "�le-du-Prince-�douard" },
                new Province { ProvinceID = "QC", Nom = "Qu�bec" },
                new Province { ProvinceID = "SK", Nom = "Saskatchewan" },
                new Province { ProvinceID = "YT", Nom = "Yukon" }
            };

            context.AddRange(provinces);
            context.SaveChanges();


        }

    }

}




app.Run();
