using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PoliceApp.Models
{
    public class Comentariu
    {
        public int ComentariuId { get; set; }
        public string? Continut { get; set; }
        public int CazId { get; set; }
        public Caz? Caz { get; set; }
    }

}
