using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoUploader_ConfettiService.Data
{
    public class VideoDbContext: DbContext 
    {
        public VideoDbContext( DbContextOptions<VideoDbContext> options) : base(options)
        {

        }

        public DbSet<Video> Videos { get; set; }
      
    }
}
