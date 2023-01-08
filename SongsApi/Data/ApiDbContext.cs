using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SongsApi.Models;
using System.Reflection;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;

namespace SongsApi.Data
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }



        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }

        public void UpdateEntity<T> (T InitialEntity, T UpdatedEntity)
        {
     
            PropertyInfo[] Props = InitialEntity.GetType().GetProperties();
            foreach (PropertyInfo prop in Props)
            {
                var PropertyValueOnUpdatedModel = prop.GetValue(UpdatedEntity, null);
                if(PropertyValueOnUpdatedModel is not null && prop.Name != "Id")
                {
                    prop.SetValue(InitialEntity, PropertyValueOnUpdatedModel, null);
                }
            }
        }

        public async Task<string> UploadImageOrAudio(IFormFile File, string AzureCn , string AzureContainerName)
        {
        
            BlobContainerClient blobContainer = new BlobContainerClient(AzureCn, AzureContainerName);  // Get a reference to the azure container
            BlobClient blobClient = blobContainer.GetBlobClient(File.FileName); 
      
            var memorystream = new MemoryStream();
            await File.CopyToAsync(memorystream);
            memorystream.Position= 0;
            await blobClient.UploadAsync(memorystream);

            return blobClient.Uri.AbsoluteUri;
        }

        
    }
}
