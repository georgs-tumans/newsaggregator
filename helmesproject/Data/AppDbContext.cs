global using helmesproject.Models;
using Microsoft.EntityFrameworkCore;

namespace helmesproject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<NewsItem> NewsItems { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
