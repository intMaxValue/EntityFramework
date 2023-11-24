

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Address")]
    public class AddressDto
    {
        [MaxLength(20)]
        [MinLength(10)]
        public string StreetName { get; set; }

        public int StreetNumber { get; set; }

        public string PostCode { get; set; }
        
        [MaxLength(15)]
        [MinLength(5)]
        public string City { get; set; }

        [MaxLength(15)]
        [MinLength(5)]
        public string Country { get; set; }
    }
}
