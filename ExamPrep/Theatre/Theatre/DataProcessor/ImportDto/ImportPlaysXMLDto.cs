

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Theatre.Data.Models.Enums;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlaysXMLDto
    {
        [Required]
        [StringLength(50, MinimumLength = 4)]
        public string Title { get; set; } 

        [Required]
        //[DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{0:hh\\:mm\\:ss}", ApplyFormatInEditMode = true)]
        //[Range(typeof(TimeSpan), "00:00:01", "23:59:59")]
        public string Duration { get; set; }

        [Required]
        [Range(0.00, 10.00)]
        [XmlElement("Raiting")]
        public float Rating { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        [StringLength(700)]
        public string Description { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string Screenwriter { get; set; }
    }
}
