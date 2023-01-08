using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using Microsoft.EntityFrameworkCore;
using SongsApi.Data;
using SongsApi.Models;

namespace SongsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : Controller
    {

        private readonly ApiDbContext _context;
        private string AzureCn;
        private string AzureImageContainerName;
        private string AzureAudioContainerName;

        public SongsController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            AzureCn = configuration.GetConnectionString("AzureStorageAccountConnection");
            AzureImageContainerName = configuration["AzureImageContainerName"];
            AzureAudioContainerName = configuration["AzureAudioContainerName"];

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return _context.Songs is null ? NotFound("No Songs Available") : Ok(await _context.Songs.Select(s => new { id = s.Id,
                Title = s.Title,
                Duration = s.Duration,
                AudioURL = s.AudioURL,
                ImageURL = s.ImageURL }).ToListAsync());
        }

        [HttpGet("NewlyAdded")]
        public async Task<IActionResult> GetNewlyAdded() //Retrun songs that were added in the last 7 days
        {
            return _context.Songs is null ? NotFound("No Songs Available") : Ok(await _context.Songs.
                Where(s => s.UploadedOn <= DateTime.Today && s.UploadedOn >= DateTime.Today.AddDays(-7)).Select(s => new {
                    id = s.Id,
                    Title = s.Title,
                    Duration = s.Duration,
                    AudioURL = s.AudioURL,
                    ImageURL = s.ImageURL
                }).ToListAsync());
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var SongToReturn = _context.Songs.Where(x => x.Id == id).FirstOrDefault();
            return SongToReturn is null ? NotFound() : Ok(SongToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Song song)
        {

            song.ImageURL = await _context.UploadImageOrAudio(song.Image, AzureCn, AzureImageContainerName);
            song.AudioURL = await _context.UploadImageOrAudio(song.Audio, AzureCn, AzureAudioContainerName);
            song.UploadedOn = DateTime.Now;
            _context.Songs.Add(song);
            _context.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var SongToDelete = _context.Songs.Where(s => s.Id == id).FirstOrDefault();
            if(SongToDelete is null) return NotFound();
            _context.Songs.Remove(SongToDelete);
            _context.SaveChanges();
            return Ok("Deleted  Successfully");
        }
    }
}
