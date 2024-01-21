using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class ExportPatientsXMLDto
    {
        [XmlElement("Name")]
        public string FullName { get; set; } = null!;

        [Required]
        public AgeGroup AgeGroup { get; set; }

        [Required]
        [XmlAttribute("Gender")]
        public string Gender { get; set; }

        public ExportMedicinesXMLDto[] Medicines { get; set; }
    }
}
