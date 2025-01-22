using Microsoft.EntityFrameworkCore;
using NoteTakingApp.Models;

namespace NoteTakingApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
