using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDespatcherDto
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        public string Name { get; set; }

        public string Position { get; set; }

        [XmlArray("Trucks")]
        [XmlArrayItem("Truck")]
        public ImportTruckDto[] Trucks { get; set; }
    }
}
