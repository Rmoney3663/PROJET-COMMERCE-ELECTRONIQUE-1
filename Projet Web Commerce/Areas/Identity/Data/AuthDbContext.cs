using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Projet_Web_Commerce.Areas.Identity.Data;
using Projet_Web_Commerce.Models;
using System.Reflection.Emit;

namespace Projet_Web_Commerce.Data;

public class AuthDbContext : IdentityDbContext<Utilisateur>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<PPArticlesEnPanier> PPArticlesEnPanier { get; set; } 

    public DbSet<PPCategories> PPCategories { get; set; }

    public DbSet<PPClients> PPClients { get; set; }

    public DbSet<PPCommandes> PPCommandes { get; set; }

    public  DbSet<PPDetailsCommandes> PPDetailsCommandes { get; set; }

    public DbSet<PPHistoriquePaiements> PPHistoriquePaiements { get; set; }



    public  DbSet<PPPoidsLivraisons> PPPoidsLivraisons { get; set; }

    public  DbSet<PPProduits> PPProduits { get; set; }


    public  DbSet<PPTaxeFederale> PPTaxeFederale { get; set; }

    public  DbSet<PPTaxeProvinciale> PPTaxeProvinciale { get; set; }

    public  DbSet<PPTypesLivraison> PPTypesLivraison { get; set; }

    public  DbSet<PPTypesPoids> PPTypesPoids { get; set; }


    public  DbSet<PPVendeurs> PPVendeurs { get; set; }

    public  DbSet<PPVendeursClients> PPVendeursClients { get; set; }
    public DbSet<PPGestionnaire> PPGestionnaire { get; set; }
    public  DbSet<Province> Province { get; set; }
    public DbSet<PPEvaluations> PPEvaluations { get; set; }

    public DbSet<PPMessages> PPMessages { get; set; }

    public DbSet<PPDestinatairesMessage> PPDestinatairesMessage { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<PPVendeurs>()
        .HasIndex(e => e.NomAffaires)
        .IsUnique();

        builder.Entity<PPVendeursClients>()
        .HasOne<PPClients>(e => e.PPClients)
        .WithMany(u => u.PPVendeursClients)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPVendeursClients>()
        .HasOne<PPVendeurs>(e => e.PPVendeurs)
        .WithMany(u => u.PPVendeursClients)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPClients>()
        .HasOne<Province>(e => e.Province)
        .WithMany(u => u.PPClients)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPVendeurs>()
        .HasOne<Province>(e => e.Province)
        .WithMany(u => u.PPVendeurs)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPArticlesEnPanier>()
        .HasOne<PPVendeurs>(e => e.PPVendeurs)
        .WithMany(u => u.PPArticlesEnPanier)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPArticlesEnPanier>()
        .HasOne<PPClients>(e => e.PPClients)
        .WithMany(u => u.PPArticlesEnPanier)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPArticlesEnPanier>()
        .HasOne<PPProduits>(e => e.PPProduits)
        .WithMany(u => u.PPArticlesEnPanier)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPArticlesEnPanier>()
        .HasOne<PPProduits>(e => e.PPProduits)
        .WithMany(u => u.PPArticlesEnPanier)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPCommandes>()
        .HasOne<PPVendeurs>(e => e.PPVendeurs)
        .WithMany(u => u.PPCommandes)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPDetailsCommandes>()
        .HasOne<PPCommandes>(e => e.PPCommandes)
        .WithMany(u => u.PPDetailsCommandes)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPDetailsCommandes>()
        .HasOne<PPProduits>(e => e.PPProduits)
        .WithMany(u => u.PPDetailsCommandes)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPProduits>()
        .HasOne<PPVendeurs>(e => e.PPVendeurs)
        .WithMany(u => u.PPProduits)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPProduits>()
        .HasOne<PPCategories>(e => e.PPCategories)
        .WithMany(u => u.PPProduits)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPCommandes>()
        .HasOne<PPTypesLivraison>(e => e.PPTypesLivraison)
        .WithMany(u => u.PPCommandes)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPPoidsLivraisons>()
        .HasOne<PPTypesLivraison>(e => e.PPTypesLivraison)
        .WithMany(u => u.PPPoidsLivraisons)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPPoidsLivraisons>()
        .HasOne<PPTypesPoids>(e => e.PPTypesPoids)
        .WithMany(u => u.PPPoidsLivraisons)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPEvaluations>()
       .HasOne<PPProduits>(e => e.PPProduits)
       .WithMany(u => u.PPEvaluations)
       .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPEvaluations>()
       .HasOne<PPClients>(e => e.PPClients)
       .WithMany(u => u.PPEvaluations)
       .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPDestinatairesMessage>()
        .HasOne<PPMessages>(e => e.Message)
        .WithMany(u => u.Destinataires)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PPVendeursClients>()
            .HasKey(e => new { e.NoVendeur, e.NoClient, e.DateVisite });

        builder.Entity<PPPoidsLivraisons>()
            .HasKey(e => new { e.CodeLivraison, e.CodePoids });

        builder.Entity<PPClients>().HasKey(x => new { x.NoClient });
        builder.Entity<PPVendeurs>().HasKey(x => new { x.NoVendeur });
        builder.Entity<PPGestionnaire>().HasKey(x => new { x.NoGestionnaire });
        builder.Entity<PPProduits>().HasKey(x => new { x.NoProduit });
        builder.Entity<PPArticlesEnPanier>().HasKey(x => new { x.NoPanier });
    }
}
