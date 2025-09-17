using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Repository
{
    public class IdentityDbContext
    {
        public IdentityDbContext(DbContextOptions<DataContext> options)
        {
            Options = options;
        }

        public DbContextOptions<DataContext> Options { get; }
    }
}