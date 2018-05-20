using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MWN.Models
{
    public class MWNContext : IdentityDbContext<User>
    {
        public MWNContext (DbContextOptions<MWNContext> options)
            : base(options)
        {
        }
       

        public DbSet<Note> Note { get; set; }
    }

}
