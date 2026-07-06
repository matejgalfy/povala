using Microsoft.AspNetCore.Identity;
using Povala.Api.Constants;
namespace Povala.Api.Models
{
    public class User : IdentityUser<int>
    {
        // We inherit UserName, is Name actually needed?
        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = AppRoles.User;
        public int? ManagedGymId { get; set; }
    }
}
