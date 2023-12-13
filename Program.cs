using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static Dictionary<string, Queue<string>> stock = new Dictionary<string, Queue<string>>();
    static Queue<string> alertes = new Queue<string>(3);

    /// <summary>
    /// Fonction principale qui initialise le stock et lance le menu interactif.
    /// </summary>
    static void Main()
    {
        AjouterProduit("B1");
        AjouterProduit("C1");
        AjouterProduit("A1");
        AjouterProduit("C2");
        AjouterProduit("C3");

        bool quitter = false;

        while (!quitter)
        {
            Console.WriteLine("\nMenu :");
            Console.WriteLine("1. Afficher le stock");
            Console.WriteLine("2. Ajouter des produits");
            Console.WriteLine("3. Gérer les alertes");
            Console.WriteLine("4. Traiter une alerte");
            Console.WriteLine("5. Sortie Colis");
            Console.WriteLine("6. Quitter");

            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AfficherStock();
                    break;

                case "2":
                    SaisieRapide();
                    break;

                case "3":
                    GererAlertes();
                    break;

                case "4":
                    TraiterAlerte();
                    break;

                case "5":
                    SortieColis();
                    break;

                case "6":
                    quitter = true;
                    break;

                default:
                    Console.WriteLine("Choix non valide. Veuillez réessayer.");
                    break;
            }
        }
    }

    /// <summary>
    /// Permet à l'utilisateur d'ajouter rapidement des produits au stock.
    /// </summary>
    static void SaisieRapide()
    {
        Console.WriteLine("Entrez une liste de produits séparés par des virgules (par exemple, A1, A2, B3) :");
        string saisie = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(saisie))
        {
            string[] produits = saisie.Split(',');

            foreach (var produit in produits)
            {
                AjouterProduit(produit.Trim());
            }
        }
    }

    /// <summary>
    /// Ajoute un produit au stock. Crée une nouvelle file d'attente si le produit n'existe pas.
    /// </summary>
    /// <param name="produit">Le produit à ajouter.</param>
    static void AjouterProduit(string produit)
    {
        if (stock.ContainsKey(produit))
        {
            stock[produit].Enqueue(produit);
        }
        else
        {
            Queue<string> nouvelleFile = new Queue<string>();
            nouvelleFile.Enqueue(produit);
            stock.Add(produit, nouvelleFile);
        }
    }


    /// <summary>
    /// Affiche le contenu du stock.
    /// </summary>
    static void AfficherStock()
    {
        Console.WriteLine("Contenu du stock :");

        foreach (var kvp in stock)
        {
            Console.Write($"File d'attente pour {kvp.Key} : ");
            Console.WriteLine(string.Join(" ", kvp.Value));
        }
    }

    /// <summary>
    /// Gère les alertes en mettant en file les produits dont la quantité est inférieure à un seuil.
    /// Traite les alertes si la file d'alertes atteint une certaine taille.
    /// </summary>
    static void GererAlertes()
    {
        const int seuil = 2;

        foreach (var kvp in stock)
        {
            if (kvp.Value.Count < seuil)
            {
                alertes.Enqueue(kvp.Key);

                if (alertes.Count == 3)
                {
                    TraiterAlertes();
                }
            }
        }

        AfficherAlertes();
    }


    /// <summary>
    /// Traite toutes les alertes en réapprovisionnant les produits nécessaires.
    /// </summary>
    static void TraiterAlertes()
    {
        const int quantiteAjoutee = 2;

        while (alertes.Count > 0)
        {
            string idProduit = alertes.Dequeue();
            for (int i = 0; i < quantiteAjoutee; i++)
            {
                AjouterProduit(idProduit);
            }

            Console.WriteLine($"Produit {idProduit} rajouté {quantiteAjoutee} fois dans le stock.");
        }
    }


    /// <summary>
    /// Traite une alerte à la demande en réapprovisionnant le produit en tête de la file d'alertes.
    /// </summary>
    static void TraiterAlerte()
    {
        if (alertes.Count > 0)
        {
            string idProduit = alertes.Dequeue();
            const int quantiteAjoutee = 2;

            for (int i = 0; i < quantiteAjoutee; i++)
            {
                AjouterProduit(idProduit);
            }

            Console.WriteLine($"Produit {idProduit} rajouté {quantiteAjoutee} fois dans le stock.");
        }
        else
        {
            Console.WriteLine("Aucune alerte disponible pour le traitement.");
        }
    }


    /// <summary>
    /// Affiche les alertes de stock dans l'ordre chronologique.
    /// </summary>
    static void AfficherAlertes()
    {
        Console.WriteLine("Alertes de stock pour les produits avec quantité inférieure à 2 :");

        if (alertes.Count > 0)
        {
            Console.WriteLine("Ordre chronologique des alertes :");
            foreach (var alerte in alertes)
            {
                Console.WriteLine($"Alerte pour le produit : {alerte}");
            }
        }
        else
        {
            Console.WriteLine("Aucune alerte. Tous les produits ont une quantité suffisante.");
        }
    }


    /// <summary>
    /// Gère la sortie de colis en demandant à l'utilisateur de spécifier les produits.
    /// </summary>
    static void SortieColis()
    {
        Console.WriteLine("Entrez une liste de produits séparés par des virgules (par exemple, A1, A2, B3) :");
        string saisie = Console.ReadLine();
        Stack<string> colis = new Stack<string>();

        if (!string.IsNullOrWhiteSpace(saisie))
        {
            string[] produits = saisie.Split(',');

            foreach (var produit in produits)
            {
                SupprimerProduit(colis, produit.Trim());
            }

            var colisTrie = colis.OrderBy(e => ExtraireChiffre(e)).ToArray();

            string resultat = string.Join(", ", colisTrie);

            Console.WriteLine($"Liste des produits du colis : {resultat}");
        }
    }

    /// <summary>
    /// Supprime un produit du stock pour l'ajouter à un colis. Gère les cas où le produit n'est plus en stock.
    /// </summary>
    /// <param name="colis">La pile représentant le colis en cours de formation.</param>
    /// <param name="produit">Le produit à supprimer du stock.</param>
    static void SupprimerProduit(Stack<string> colis, string produit)
    {
        if (stock.ContainsKey(produit) && stock[produit].Count > 0)
        {
            colis.Push(stock[produit].Dequeue());
        }
        else
        {
            Console.WriteLine($"{produit} : Plus de stock pour ce produit, impossible de l'ajouter au colis.");
        }
    }

    /// <summary>
    /// Extrait et retourne le premier chiffre trouvé dans une chaîne.
    /// </summary>
    /// <param name="s">La chaîne à analyser.</param>
    /// <returns>Le premier chiffre trouvé, ou -1 si aucun chiffre n'est présent.</returns>
    static int ExtraireChiffre(string s)
    {
        foreach (char c in s)
        {
            if (char.IsDigit(c))
            {
                return int.Parse(c.ToString());
            }
        }
        return -1;
    }
}