using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType("Creator")]
    public class ImportCreatorXMLDto
    {
        [Required]
        [XmlElement("FirstName")]
        [StringLength(7, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [XmlElement("LastName")]
        [StringLength(7, MinimumLength = 2)]
        public string LastName { get; set; } = null!;

        [XmlArray("Boardgames")]
        [XmlArrayItem("Boardgame")]
        public ImportBoardgameXMLDto[] Boardgames { get; set; } 
    }
}
