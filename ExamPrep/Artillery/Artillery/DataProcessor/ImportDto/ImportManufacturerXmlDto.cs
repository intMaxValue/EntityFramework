using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Manufacturer")]
    public class ImportManufacturerXmlDto
    {
        [Required]
        [XmlElement("ManufacturerName")]
        [StringLength(40, MinimumLength = 4)]
        // unique?
        public string ManufacturerName { get; set; }

        [Required]
        [XmlElement("Founded")]
        [StringLength(100, MinimumLength = 10)]
        public string Founded { get; set; }
    }
}
