namespace PoliceApp.Models
{
    public class Caz
    {
        public int CazId { get; set; }
        public string? Descriere { get; set; }
        public string? Stadiu { get; set; }
        public string? Prioritate { get; set; }
        public int UtilizatorId { get; set; }
        public bool Arhivat { get; set; } // Adaugă proprietatea Arhivat
        public virtual Utilizator? Utilizator { get; set; }
        public virtual ICollection<Comentariu>? Comentarii { get; set; } // Adăugat pentru comentarii
    }
}
