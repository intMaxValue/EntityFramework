using System.ComponentModel.DataAnnotations;


namespace Invoices.Data.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(25, MinimumLength = 10)]
        public string Name { get; set; } = null!;
        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string NumberVat { get; set; }
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<ProductClient> ProductsClients { get; set; } = new List<ProductClient>();
    }
}
