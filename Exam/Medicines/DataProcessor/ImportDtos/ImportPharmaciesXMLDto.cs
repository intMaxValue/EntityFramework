using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    [XmlType("Pharmacy")]
    public class ImportPharmaciesXMLDto
    {
        [Required]
        [XmlElement("Name")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [XmlElement("PhoneNumber")]
        [RegularExpression(@"\(\d{3}\) \d{3}-\d{4}")]
        public string PhoneNumber { get; set; }

        [Required]
        [XmlAttribute("non-stop")]
        public string IsNonStop { get; set; }

        [XmlArray("Medicines")]
        [XmlArrayItem("Medicine")]
        public ImportMedicinesXMLDto[] Medicines { get; set; }
    }
}
