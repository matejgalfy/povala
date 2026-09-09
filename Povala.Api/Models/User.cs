using Microsoft.AspNetCore.Identity;
using Povala.Shared.Constants;

namespace Povala.Api.Models
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; } = string.Empty;

        public string Role { get; set; } = AppRoles.User;
    }
}
