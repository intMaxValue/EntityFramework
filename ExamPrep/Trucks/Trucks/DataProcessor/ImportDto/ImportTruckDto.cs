using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckDto
    {
        [MaxLength(8)]
        [MinLength(8)]
        public string RegistrationNumber { get; set; }

        [Required]
        [MaxLength(17)]
        [MinLength(17)]
        public string VinNumber { get; set; }

        [Range(950, 1420)]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        public int CargoCapacity { get; set; }

        [Required]
        public int CategoryType { get; set; }

        [Required]
        public int MakeType { get; set; }
    }
}
