using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Country")]
    public class ImportCountriesXmlDto
    {
        [Required]
        [StringLength(60, MinimumLength = 4)]
        public string CountryName { get; set; } = null!;

        [Required]
        [Range(50000, 10000000)]
        public int ArmySize { get; set; }
    }
}
