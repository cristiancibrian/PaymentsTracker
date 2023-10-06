using Microsoft.EntityFrameworkCore;

namespace PaymentsTracker.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }
        public DbSet<Payment> Payments { get; set; }
    }
}
