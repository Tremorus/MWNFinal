using Microsoft.AspNetCore.Identity;

namespace MWN.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Login { get; set; }

    }
}
