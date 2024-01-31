using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Data;
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var rolemanagerIdentity = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //var rolemanagerCustom = scope.ServiceProvider.GetRequiredService<RoleManager<TypesUtilisateur>>();

    var roles = new[] { "Gestionnaire", "Acheteur", "Vendeur" };
    var types = new[] { "G", "A", "V" };


    for (var i = 0; i < roles.Length; i++)
    {
        if (!await rolemanagerIdentity.RoleExistsAsync(roles[i]))
        {
            var newRole = new IdentityRole(roles[i]);
            await rolemanagerIdentity.CreateAsync(newRole);
        }
    }
}

app.Run();
