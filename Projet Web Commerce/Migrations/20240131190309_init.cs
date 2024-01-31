using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projet_Web_Commerce.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPCategories",
                columns: table => new
                {
                    NoCategorie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPCategories", x => x.NoCategorie);
                });

            migrationBuilder.CreateTable(
                name: "PPHistoriquePaiements",
                columns: table => new
                {
                    NoHistorique = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MontantVenteAvantLivraison = table.Column<decimal>(type: "smallmoney", nullable: false),
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    NoCommande = table.Column<int>(type: "int", nullable: false),
                    DateVente = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoAutorisation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FraisLesi = table.Column<decimal>(type: "smallmoney", nullable: false),
                    Redevance = table.Column<decimal>(type: "smallmoney", nullable: false),
                    FraisLivraison = table.Column<decimal>(type: "smallmoney", nullable: false),
                    FraisTPS = table.Column<decimal>(type: "smallmoney", nullable: false),
                    FraisTVQ = table.Column<decimal>(type: "smallmoney", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPHistoriquePaiements", x => x.NoHistorique);
                });

            migrationBuilder.CreateTable(
                name: "PPTaxeFederale",
                columns: table => new
                {
                    NoTPS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateEffectiveTPS = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TauxTPS = table.Column<decimal>(type: "numeric(4,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPTaxeFederale", x => x.NoTPS);
                });

            migrationBuilder.CreateTable(
                name: "PPTaxeProvinciale",
                columns: table => new
                {
                    NoTVQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateEffectiveTVQ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TauxTVQ = table.Column<decimal>(type: "numeric(5,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPTaxeProvinciale", x => x.NoTVQ);
                });

            migrationBuilder.CreateTable(
                name: "PPTypesLivraison",
                columns: table => new
                {
                    CodeLivraison = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPTypesLivraison", x => x.CodeLivraison);
                });

            migrationBuilder.CreateTable(
                name: "PPTypesPoids",
                columns: table => new
                {
                    CodePoids = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoidsMin = table.Column<decimal>(type: "numeric(8,1)", nullable: false),
                    PoidsMax = table.Column<decimal>(type: "numeric(8,1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPTypesPoids", x => x.CodePoids);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    ProvinceID = table.Column<string>(type: "char(2)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.ProvinceID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPPoidsLivraisons",
                columns: table => new
                {
                    CodeLivraison = table.Column<int>(type: "int", nullable: false),
                    CodePoids = table.Column<int>(type: "int", nullable: false),
                    Tarif = table.Column<decimal>(type: "smallmoney", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPPoidsLivraisons", x => new { x.CodeLivraison, x.CodePoids });
                    table.ForeignKey(
                        name: "FK_PPPoidsLivraisons_PPTypesLivraison_CodeLivraison",
                        column: x => x.CodeLivraison,
                        principalTable: "PPTypesLivraison",
                        principalColumn: "CodeLivraison",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPPoidsLivraisons_PPTypesPoids_CodePoids",
                        column: x => x.CodePoids,
                        principalTable: "PPTypesPoids",
                        principalColumn: "CodePoids",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPClients",
                columns: table => new
                {
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateMAJ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NbConnexions = table.Column<int>(type: "int", nullable: false),
                    DateDerniereConnexion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    AdresseEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoProvince = table.Column<string>(type: "char(2)", nullable: false),
                    CodePostal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUtilisateur = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPClients", x => x.NoClient);
                    table.ForeignKey(
                        name: "FK_PPClients_AspNetUsers_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPClients_Province_NoProvince",
                        column: x => x.NoProvince,
                        principalTable: "Province",
                        principalColumn: "ProvinceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPVendeurs",
                columns: table => new
                {
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    NomAffaires = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoidsMaxLivraison = table.Column<int>(type: "int", nullable: false),
                    LivraisonGratuite = table.Column<decimal>(type: "smallmoney", nullable: false),
                    Taxes = table.Column<bool>(type: "bit", nullable: false),
                    Pourcentage = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateMAJ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    AdresseEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoProvince = table.Column<string>(type: "char(2)", nullable: false),
                    CodePostal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUtilisateur = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPVendeurs", x => x.NoVendeur);
                    table.ForeignKey(
                        name: "FK_PPVendeurs_AspNetUsers_IdUtilisateur",
                        column: x => x.IdUtilisateur,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPVendeurs_Province_NoProvince",
                        column: x => x.NoProvince,
                        principalTable: "Province",
                        principalColumn: "ProvinceID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelephoneClients",
                columns: table => new
                {
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelephoneClients", x => x.NoClient);
                    table.ForeignKey(
                        name: "FK_TelephoneClients_PPClients_NoClient",
                        column: x => x.NoClient,
                        principalTable: "PPClients",
                        principalColumn: "NoClient",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPCommandes",
                columns: table => new
                {
                    NoCommande = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    DateCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PoidsTotal = table.Column<decimal>(type: "numeric(8,1)", nullable: false),
                    Statut = table.Column<string>(type: "char(1)", nullable: false),
                    NoAutorisation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MontantTotAvantTaxes = table.Column<decimal>(type: "smallmoney", nullable: false),
                    TPS = table.Column<decimal>(type: "smallmoney", nullable: false),
                    TVQ = table.Column<decimal>(type: "smallmoney", nullable: false),
                    CoutLivraison = table.Column<decimal>(type: "smallmoney", nullable: false),
                    TypeLivraison = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPCommandes", x => x.NoCommande);
                    table.ForeignKey(
                        name: "FK_PPCommandes_PPClients_NoClient",
                        column: x => x.NoClient,
                        principalTable: "PPClients",
                        principalColumn: "NoClient",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PPCommandes_PPTypesLivraison_TypeLivraison",
                        column: x => x.TypeLivraison,
                        principalTable: "PPTypesLivraison",
                        principalColumn: "CodeLivraison",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPCommandes_PPVendeurs_NoVendeur",
                        column: x => x.NoVendeur,
                        principalTable: "PPVendeurs",
                        principalColumn: "NoVendeur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPProduits",
                columns: table => new
                {
                    NoProduit = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    NoCategorie = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateMAJ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrixDemande = table.Column<decimal>(type: "smallmoney", nullable: false),
                    Disponibilite = table.Column<bool>(type: "bit", nullable: false),
                    Poids = table.Column<decimal>(type: "numeric(8,1)", nullable: false),
                    DateVente = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrixVente = table.Column<decimal>(type: "smallmoney", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPProduits", x => x.NoProduit);
                    table.ForeignKey(
                        name: "FK_PPProduits_PPCategories_NoCategorie",
                        column: x => x.NoCategorie,
                        principalTable: "PPCategories",
                        principalColumn: "NoCategorie",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPProduits_PPVendeurs_NoVendeur",
                        column: x => x.NoVendeur,
                        principalTable: "PPVendeurs",
                        principalColumn: "NoVendeur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPVendeursClients",
                columns: table => new
                {
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    DateVisite = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPVendeursClients", x => new { x.NoVendeur, x.NoClient, x.DateVisite });
                    table.ForeignKey(
                        name: "FK_PPVendeursClients_PPClients_NoClient",
                        column: x => x.NoClient,
                        principalTable: "PPClients",
                        principalColumn: "NoClient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPVendeursClients_PPVendeurs_NoVendeur",
                        column: x => x.NoVendeur,
                        principalTable: "PPVendeurs",
                        principalColumn: "NoVendeur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TelephoneVendeurs",
                columns: table => new
                {
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelephoneVendeurs", x => x.NoVendeur);
                    table.ForeignKey(
                        name: "FK_TelephoneVendeurs_PPVendeurs_NoVendeur",
                        column: x => x.NoVendeur,
                        principalTable: "PPVendeurs",
                        principalColumn: "NoVendeur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPArticlesEnPanier",
                columns: table => new
                {
                    NoPanier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoClient = table.Column<int>(type: "int", nullable: false),
                    NoVendeur = table.Column<int>(type: "int", nullable: false),
                    NoProduit = table.Column<int>(type: "int", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NbItems = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPArticlesEnPanier", x => x.NoPanier);
                    table.ForeignKey(
                        name: "FK_PPArticlesEnPanier_PPClients_NoClient",
                        column: x => x.NoClient,
                        principalTable: "PPClients",
                        principalColumn: "NoClient",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPArticlesEnPanier_PPProduits_NoProduit",
                        column: x => x.NoProduit,
                        principalTable: "PPProduits",
                        principalColumn: "NoProduit",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPArticlesEnPanier_PPVendeurs_NoVendeur",
                        column: x => x.NoVendeur,
                        principalTable: "PPVendeurs",
                        principalColumn: "NoVendeur",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PPDetailsCommandes",
                columns: table => new
                {
                    NoDetailsCommande = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoCommande = table.Column<int>(type: "int", nullable: false),
                    NoProduit = table.Column<int>(type: "int", nullable: false),
                    PrixVente = table.Column<decimal>(type: "smallmoney", nullable: false),
                    Quantité = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPDetailsCommandes", x => x.NoDetailsCommande);
                    table.ForeignKey(
                        name: "FK_PPDetailsCommandes_PPCommandes_NoCommande",
                        column: x => x.NoCommande,
                        principalTable: "PPCommandes",
                        principalColumn: "NoCommande",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPDetailsCommandes_PPProduits_NoProduit",
                        column: x => x.NoProduit,
                        principalTable: "PPProduits",
                        principalColumn: "NoProduit",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PPArticlesEnPanier_NoClient",
                table: "PPArticlesEnPanier",
                column: "NoClient");

            migrationBuilder.CreateIndex(
                name: "IX_PPArticlesEnPanier_NoProduit",
                table: "PPArticlesEnPanier",
                column: "NoProduit");

            migrationBuilder.CreateIndex(
                name: "IX_PPArticlesEnPanier_NoVendeur",
                table: "PPArticlesEnPanier",
                column: "NoVendeur");

            migrationBuilder.CreateIndex(
                name: "IX_PPClients_IdUtilisateur",
                table: "PPClients",
                column: "IdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_PPClients_NoProvince",
                table: "PPClients",
                column: "NoProvince");

            migrationBuilder.CreateIndex(
                name: "IX_PPCommandes_NoClient",
                table: "PPCommandes",
                column: "NoClient");

            migrationBuilder.CreateIndex(
                name: "IX_PPCommandes_NoVendeur",
                table: "PPCommandes",
                column: "NoVendeur");

            migrationBuilder.CreateIndex(
                name: "IX_PPCommandes_TypeLivraison",
                table: "PPCommandes",
                column: "TypeLivraison");

            migrationBuilder.CreateIndex(
                name: "IX_PPDetailsCommandes_NoCommande",
                table: "PPDetailsCommandes",
                column: "NoCommande");

            migrationBuilder.CreateIndex(
                name: "IX_PPDetailsCommandes_NoProduit",
                table: "PPDetailsCommandes",
                column: "NoProduit");

            migrationBuilder.CreateIndex(
                name: "IX_PPPoidsLivraisons_CodePoids",
                table: "PPPoidsLivraisons",
                column: "CodePoids");

            migrationBuilder.CreateIndex(
                name: "IX_PPProduits_NoCategorie",
                table: "PPProduits",
                column: "NoCategorie");

            migrationBuilder.CreateIndex(
                name: "IX_PPProduits_NoVendeur",
                table: "PPProduits",
                column: "NoVendeur");

            migrationBuilder.CreateIndex(
                name: "IX_PPVendeurs_IdUtilisateur",
                table: "PPVendeurs",
                column: "IdUtilisateur");

            migrationBuilder.CreateIndex(
                name: "IX_PPVendeurs_NoProvince",
                table: "PPVendeurs",
                column: "NoProvince");

            migrationBuilder.CreateIndex(
                name: "IX_PPVendeursClients_NoClient",
                table: "PPVendeursClients",
                column: "NoClient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "PPArticlesEnPanier");

            migrationBuilder.DropTable(
                name: "PPDetailsCommandes");

            migrationBuilder.DropTable(
                name: "PPHistoriquePaiements");

            migrationBuilder.DropTable(
                name: "PPPoidsLivraisons");

            migrationBuilder.DropTable(
                name: "PPTaxeFederale");

            migrationBuilder.DropTable(
                name: "PPTaxeProvinciale");

            migrationBuilder.DropTable(
                name: "PPVendeursClients");

            migrationBuilder.DropTable(
                name: "TelephoneClients");

            migrationBuilder.DropTable(
                name: "TelephoneVendeurs");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PPCommandes");

            migrationBuilder.DropTable(
                name: "PPProduits");

            migrationBuilder.DropTable(
                name: "PPTypesPoids");

            migrationBuilder.DropTable(
                name: "PPClients");

            migrationBuilder.DropTable(
                name: "PPTypesLivraison");

            migrationBuilder.DropTable(
                name: "PPCategories");

            migrationBuilder.DropTable(
                name: "PPVendeurs");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
