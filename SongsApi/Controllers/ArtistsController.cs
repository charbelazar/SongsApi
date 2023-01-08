using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SongsApi.Data;
using SongsApi.Models;

namespace SongsApi.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class ArtistsController : Controller
    {
        private readonly ApiDbContext _context;
        private string AzureCn;
        private string AzureImageContainerName;

        public ArtistsController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            AzureCn = configuration.GetConnectionString("AzureStorageAccountConnection");
            AzureImageContainerName = configuration["AzureImageContainerName"];
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return _context.Artists != null ?
                        Ok(await _context.Artists.Include(s=> s.Songs).Include(s=>s.Albums).ToListAsync()) :
                        NotFound("No Artists Are Added");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ArtististToReturn = await _context.Artists.Where(x => x.Id == id).Include(s => s.Songs).Include(s => s.Albums).FirstOrDefaultAsync();
            if (ArtististToReturn is null)
                return NotFound();
            else
                return Ok(ArtististToReturn);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Artist artist)
        {
         if(artist.Image is not null)
            {
                artist.ImageURL = await _context.UploadImageOrAudio(artist.Image,AzureCn , AzureImageContainerName);
            }
            _context.Artists.Add(artist);
             await  _context.SaveChangesAsync();
            return Ok("The Artist Has been added");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ArtistToDelete = _context.Artists.Where(art => art.Id == id).FirstOrDefault();
            if (ArtistToDelete != null)
            {
                _context.Artists.Remove(ArtistToDelete);
                await _context.SaveChangesAsync();
                return Ok("Deleted Successfully");
            }
            else
            {
                return NotFound("No Artist Exists With This Code");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id , [FromBody] Artist UpdatedEntity)
        {
            var InitalArtist = _context.Artists.Where(x => x.Id == id).FirstOrDefault();
            if(InitalArtist is null)
            {
                return NotFound("No Artist Exists With This ID");
            }
            else
            {
                _context.UpdateEntity(InitalArtist, UpdatedEntity);

                _context.SaveChanges();
                return Ok("Successfully Updated");
            }
        }
    }
}
