using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class StatContext: DbContext
    {
        public StatContext(DbContextOptions<StatContext> opt): base(opt)
        {
            
        }

        public DbSet<Stat> Stats { get; set; }
    }
    
}