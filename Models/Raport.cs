namespace PoliceApp.Models
{
    public class Raport
    {
        public int RaportId { get; set; }
        public string? Continut { get; set; }
        public int CazId { get; set; }
        public virtual Caz? Caz { get; set; }
    }
}
