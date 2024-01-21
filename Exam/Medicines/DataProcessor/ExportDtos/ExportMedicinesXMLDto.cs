using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Medicine")]
    public class ExportMedicinesXMLDto
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }

        [XmlAttribute("Category")]
        public string Category { get; set; }

        [XmlElement("Producer")]
        public string Producer { get; set; } = null!;

        [XmlIgnore]
        public DateTime ExpiryDate { get; set; }

        [XmlElement("BestBefore")]
        public string FormattedExpiryDate
        {
            get => ExpiryDate.ToString("yyyy-MM-dd");
            set => ExpiryDate = DateTime.ParseExact(value, "yyyy-MM-dd", null);
        }

    }
}
