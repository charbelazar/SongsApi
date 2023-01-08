using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace SongsApi.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Language { get; set; } 
        public string Duration { get; set; }
        public DateTime? UploadedOn { get; set;}
        public bool IsFeatured { get; set;}
        public string? ImageURL { get; set; }
        public string? AudioURL { get; set; }
        public int ArtistId { get; set; }
        public int? AlbumId { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        [NotMapped]
        public IFormFile Audio { get; set; }
    }
}
