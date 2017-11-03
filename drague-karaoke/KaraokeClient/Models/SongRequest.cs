using System.ComponentModel.DataAnnotations;

namespace KaraokeClient.Models
{
    public class SongRequest
    {
        public int songId { get; set; }
        [Required(ErrorMessage ="La chanson est invalide")]
        public string title { get; set; }
        [Required(ErrorMessage="Votre nom est obligatoire")]
        public string singerName { get; set; }
        public string notes { get; set; }
    }

    public class Category
    {
        string categoryName { get; set; }
    }
}
