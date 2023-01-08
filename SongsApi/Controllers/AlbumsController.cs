using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SongsApi.Data;
using SongsApi.Models;

namespace SongsApi.Controllers
{

    [Route("Api/[controller]")]
    [ApiController]
    public class AlbumsController : Controller
    {
        private readonly ApiDbContext _context;
        private string AzureCn;
        private string AzureImageContainerName;


        public AlbumsController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            AzureCn = configuration.GetConnectionString("AzureStorageAccountConnection");
            AzureImageContainerName = configuration["AzureImageContainerName"];
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return _context.Artists != null ?
                        Ok(await _context.Albums.ToListAsync()) :
                        NotFound("No Albums Are Added");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var AlbumToReturn = await _context.Albums.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (AlbumToReturn is null)
                return NotFound();
            else
                return Ok(AlbumToReturn);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Album album)
        {
            if (album.Image is not null)
            {
                album.ImageURL = await _context.UploadImageOrAudio(album.Image, AzureCn, AzureImageContainerName);
            }
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var AlbumToDelete = _context.Albums.Where(alb => alb.Id == id).FirstOrDefault();
            if (AlbumToDelete != null)
            {
                _context.Albums.Remove(AlbumToDelete);
                await _context.SaveChangesAsync();
                return Ok("Deleted Successfully");
            }
            else
            {
                return NotFound("No Album Exists With This Code");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] Album UpdatedAlbum)
        {
           
            var InitalAlbum = _context.Albums.Where(_ => _.Id == id).FirstOrDefault();
            if (InitalAlbum is null) return  NotFound("No Album Exists With This Code");

            _context.UpdateEntity(InitalAlbum, UpdatedAlbum);
            _context.SaveChanges();
            return Ok("Successfully Updated");
        }

    }
}
