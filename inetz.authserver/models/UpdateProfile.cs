using Microsoft.EntityFrameworkCore;

namespace inetz.authserver.models
{
    [Keyless]
    public class UpdateProfile
    {
        public string UserId { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
