using System.Globalization;
using Invoices.Data.Models;
using Invoices.DataProcessor.ExportDto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using System.Text;
    using System.Xml.Serialization;

    public class Serializer
    {

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var productsWithClients = context.Products
                .Include(p => p.ProductsClients)
                .ThenInclude(pc => pc.Client)
                .Where(p => p.ProductsClients.Any(pc => pc.Client.Name.Length >= nameLength))
                .ToArray();

            var result = productsWithClients
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price.ToString("F2"),
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                        .Where(pc => pc.Client.Name.Length >= nameLength)
                        .Select(pc => new
                        {
                            Name = pc.Client.Name,
                            NumberVat = pc.Client.NumberVat
                        })
                        .OrderBy(pc => pc.Name)
                        .ToArray()
                })
                .OrderByDescending(p => p.Clients.Length)
                .ThenBy(p => p.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(result, Formatting.Indented);

        }
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var clientsWithInvoices = context.Clients
                .Include(c => c.Invoices)
                .Where(c => c.Invoices.Any(i => i.IssueDate > date))
                .ToArray();

            var result = clientsWithInvoices
                .Select(c => new ExportClientWithInvoicesDto
                {
                    ClientName = c.Name,
                    VatNumber = c.NumberVat,
                    Invoices = c.Invoices
                        .Where(i => i.IssueDate > date)
                        .Select(i => new ExportInvoiceDto
                        {
                            InvoiceNumber = i.Number.ToString(),
                            InvoiceAmount = i.Amount.ToString("F2", CultureInfo.InvariantCulture),
                            DueDate = i.DueDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                            Currency = i.CurrencyType.ToString()
                        })
                        .OrderBy(i => i.IssueDate)
                        .ThenByDescending(i => i.DueDate)
                        .ToArray()
                })
                .OrderByDescending(c => c.Invoices.Length)
                .ThenBy(c => c.ClientName)
                .ToArray();

            return MySerializer<ExportClientWithInvoicesDto[]>(result, "Clients");
        }

        //SERIALIZER TO XML
        private static string MySerializer<T>(T dataTransferObjects, string xmlRootAttributeName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttributeName));
            StringBuilder sb = new StringBuilder();
            using var write = new StringWriter(sb);
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);
            serializer.Serialize(write, dataTransferObjects, xmlNamespaces);
            return sb.ToString();
        }
    }
}