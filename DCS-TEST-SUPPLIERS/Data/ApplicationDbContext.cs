using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DCS_TEST_SUPPLIERS.Models;

namespace DCS_TEST_SUPPLIERS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DCS_TEST_SUPPLIERS.Models.Attraction> Attraction { get; set; } = default!;
        public DbSet<DCS_TEST_SUPPLIERS.Models.Flight> Flight { get; set; } = default!;
        public DbSet<DCS_TEST_SUPPLIERS.Models.Hotel> Hotel { get; set; } = default!;
    }
}
