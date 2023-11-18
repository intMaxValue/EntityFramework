
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("sale")]
    public class exportSalesDto
    {
        [XmlElement("car")]
        public exportCarDto Car { get; set; }

        [XmlElement("discount")]
        public string Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public string Price { get; set; }

        [XmlElement("price-with-discount")]
        public string PriceWithDiscount { get; set; }
    }
}
