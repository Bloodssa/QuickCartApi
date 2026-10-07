namespace QuickCartApi.Entities
{
    public class Address : BaseEntity
    {
        public int UserId { get; set; }
        public string RecipientName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string Barangay { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public User User { get; set; } = null!;
    }
}
