using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType("Client")]
    public class ExportClientWithInvoicesDto
    {
        [XmlAttribute("InvoicesCount")]
        public int InvoicesCount => Invoices?.Length ?? 0;

        [XmlElement("ClientName")]
        public string ClientName { get; set; }

        [XmlElement("VatNumber")]
        public string VatNumber { get; set; }

        [XmlArray("Invoices")]
        [XmlArrayItem("Invoice")]
        public ExportInvoiceDto[] Invoices { get; set; }
    }
}
