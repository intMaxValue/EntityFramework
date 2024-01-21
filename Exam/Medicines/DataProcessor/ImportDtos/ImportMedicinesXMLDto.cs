using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Medicine")]
    public class ImportMedicinesXMLDto
    {
        [Required]
        [XmlElement("Name")]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [XmlElement("Price")]
        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        [Required]
        [XmlAttribute("category")]
        public int Category { get; set; }

        [XmlElement("ProductionDate")]
        [Required]
        public string ProductionDate { get; set; }

        [Required]
        [XmlElement("ExpiryDate")]
        public string ExpiryDate { get; set; }

        [Required]
        [XmlElement("Producer")]
        [StringLength(100, MinimumLength = 3)]
        public string Producer { get; set; } = null!;
    }
}
