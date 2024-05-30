namespace PoliceApp.Models
{
    public class Angajat
    {
        public int AngajatId { get; set; }
        public string? Nume { get; set; }
        public string? Pozitie { get; set; }
        public int UtilizatorId { get; set; }
        public virtual Utilizator? Utilizator { get; set; }
    }
}
