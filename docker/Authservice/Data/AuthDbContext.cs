using Microsoft.EntityFrameworkCore;
using Authservice.Models;

namespace Authservice.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}


