using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Invoice")]
    public class ExportInvoiceDto
    {
        [XmlElement("InvoiceNumber")]
        public string InvoiceNumber { get; set; }

        [XmlElement("InvoiceAmount")]
        public string InvoiceAmount { get; set; }

        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [XmlElement("Currency")]
        public string Currency { get; set; }

        [XmlIgnore]
        public string IssueDate { get; set; }
    }
}
