
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ClientsDto
    {
        [XmlElement("Name")]
        [MaxLength(25)]
        [MinLength(10)]
        public string Name { get; set; }

        [XmlElement("NumberVat")]
        [MaxLength(15)]
        [MinLength(10)]
        public string NumberVat { get; set; }

        [XmlArray]
        [XmlArrayItem("Address")]
        public AddressDto[] Addresses { get; set; }
    }
}
