
using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ProductDto
    {
        [MaxLength(30)]
        [MinLength(9)]
        public string Name { get; set; }
        
        [Range(5.00, 1000.00)]
        public decimal Price { get; set; }
        
        public CategoryType CategoryType { get; set; }

        public int[] Clients { get; set; }
    }
}
