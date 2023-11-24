using System.Text;
using Invoices.Data.Models;
using Invoices.DataProcessor.ImportDto;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using Invoices.Data;
    using Invoices.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            var clientsDto = MyDeserializer<ClientsDto[]>(xmlString, "Clients");
            List<Client> clients = new List<Client>();
            var sb = new StringBuilder();

            foreach (var c in clientsDto)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                Client client = new Client()
                {
                    Name = c.Name,
                    NumberVat = c.NumberVat
                };

                foreach (var a in c.Addresses)
                {
                    if (!IsValid(a))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }

                    Address address = new Address()
                    {
                        StreetName = a.StreetName,
                        StreetNumber = a.StreetNumber,
                        PostCode = a.PostCode,
                        City = a.City,
                        Country = a.Country
                    };

                    client.Addresses.Add(address);
                }
                clients.Add(client);
                sb.AppendLine($"Successfully imported client {client.Name}.");
            }
            context.Clients.AddRange(clients);
            context.SaveChanges();

            return sb.ToString();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            var sb = new StringBuilder();
            Invoice[] invoicesJson = JsonConvert.DeserializeObject<Invoice[]>(jsonString);
            var invoices = new List<Invoice>();

            foreach (var i in invoicesJson)
            {
                if (!IsValid(i))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                Invoice invoice = new Invoice();
                invoice.Number = i.Number;

                if (i.IssueDate > i.DueDate)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                invoice.IssueDate = i.IssueDate;
                invoice.DueDate = i.DueDate;
                invoice.Amount = i.Amount;

                if (!Enum.IsDefined(typeof(CurrencyType), i.CurrencyType))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                invoice.CurrencyType = i.CurrencyType;

                invoice.ClientId = context.Clients.Any(c => c.Id == i.ClientId) ? i.ClientId : 0;

                if (invoice.ClientId == 0)
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                sb.AppendLine($"Successfully imported invoice with number {invoice.Number}.");
                invoices.Add(invoice);
            }
            context.Invoices.AddRange(invoices);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var products = new List<Product>();
            var productsJson = JsonConvert.DeserializeObject<ProductDto[]>(jsonString);

            foreach (var p in productsJson)
            {
                if (!IsValid(p))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                var product = new Product();
                product.Name = p.Name;
                product.Price = p.Price;
                
                if (!Enum.IsDefined(typeof(CategoryType), p.CategoryType))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }
                product.CategoryType = p.CategoryType;

                foreach (var c in p.Clients.Distinct())
                {
                    var pc = new ProductClient
                    {
                        ClientId = c
                    };

                    if (!context.Clients.Any(c => c.Id == pc.ClientId))
                    {
                        sb.AppendLine("Invalid data!");
                        continue;
                    }


                    product.ProductsClients.Add(pc);
                }

                products.Add(product);
                sb.AppendLine(
                    $"Successfully imported product - {product.Name} with {product.ProductsClients.Count()} clients.");
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return sb.ToString();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

        //DESERIALIZER From XML
        private static T MyDeserializer<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), root);
            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)serializer.Deserialize(reader);
            return dtos;
        }

        
    } 
}
