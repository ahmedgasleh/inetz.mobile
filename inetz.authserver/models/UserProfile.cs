namespace inetz.authserver.models
{
    public partial class UserProfile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;

        public string UserEmail { get; set; } = string.Empty;
        public string UserPassWord { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

    }
}
