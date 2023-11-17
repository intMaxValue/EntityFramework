
using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
        [XmlType("Car")]
    public class carsDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("traveledDistance")]
        public long TraveledDistance { get; set; }

        [XmlArray("parts")]
        public ImportPartIdDto[] PartsIds { get; set; }

    }
}
