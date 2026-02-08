using Microsoft.AspNetCore.Identity;

namespace dotnet_core_authentication.Data
{
    public class ApplicationUsers : IdentityUser
    {
        public string Name { get; internal set; }
    }
}
