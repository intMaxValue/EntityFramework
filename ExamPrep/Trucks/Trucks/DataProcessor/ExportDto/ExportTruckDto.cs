

using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Truck")]
    public class ExportTruckDto
    {
        [XmlElement]
        public string RegistrationNumber { get; set; }

        [XmlElement]
        public string Make { get; set; }
    }
}
