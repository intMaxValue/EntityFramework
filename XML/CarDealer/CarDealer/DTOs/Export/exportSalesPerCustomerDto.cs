
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("customer")]
    public class exportSalesPerCustomerDto
    {
        [XmlAttribute("full-name")]
        public string FullName { get; set; }

        [XmlAttribute("bought-cars")]
        public int BoughtCars { get; set; }

        [XmlAttribute("spent-money")]
        public string SpentMoney { get; set; }

        [XmlIgnore]
        public bool IsYoungDriver { get; set; }
    }
}
