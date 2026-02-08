using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dotnet_core_authentication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUsers>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

            
        }
    }
}
