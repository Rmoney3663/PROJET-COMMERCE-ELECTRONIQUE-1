

using Projet_Web_Commerce.Areas.Identity.Data;

namespace Projet_Web_Commerce.Models
{
    public class ModelListeVendeurs
    {
        //public Dictionary<List<int>, PPVendeurs> VendeursList { get; set; }


        public List<PPEvaluations> EvaluationList { get; set; }
        public List<PPVendeurs> VendeursList { get; set; }

        public List<PPProduits> ProduitsList { get; set; }

        public List<ModelMoisAnnees> MoisAnneesDistinctsList { get; set; }

        public List<PPCommandes> CommandesList { get; set; }

        public List<Utilisateur> UtilisateurList { get; set; }
        public List<List<decimal?>> Redevances { get; set; }
    }
}
