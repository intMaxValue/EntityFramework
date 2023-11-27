using Footballers.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Footballers.DataProcessor.ImportDto
{
    [XmlType("Coach")]
    public class ImportCoachesXmlDto
    {
        [Required]
        [XmlElement("Name")]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [XmlElement("Nationality")]
        public string Nationality { get; set; }

        [XmlArray("Footballers")]
        [XmlArrayItem("Footballer")]
        public ImportFootballersXmlDto[] Footballers { get; set; }
    }
}
