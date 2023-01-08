using System.ComponentModel.DataAnnotations.Schema;

namespace SongsApi.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? ImageURL { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public ICollection<Album>? Albums { get; set;}
        public ICollection<Song>? Songs { get; set; }

    }
}
