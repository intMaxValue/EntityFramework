using System.Globalization;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Medicines.Data.Models.Enums;
using Medicines.DataProcessor.ExportDtos;
using Newtonsoft.Json;

namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using System.Diagnostics;
    using System.Text;
    using System.Xml.Serialization;
    using System.Xml;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            var parsedDate = DateTime.Parse(date, CultureInfo.InvariantCulture);

            var xml = context.Patients
                .Where(p => p.PatientsMedicines.Any(m => m.Medicine.ProductionDate > parsedDate))
                .Select(p => new ExportPatientsXMLDto
                {
                    FullName = p.FullName,
                    AgeGroup = p.AgeGroup,
                    Gender = p.Gender.ToString().ToLower(),
                    Medicines = p.PatientsMedicines
                        .Where(m => m.Medicine.ProductionDate > parsedDate)
                        .AsEnumerable() // Switch to client-side evaluation
                        .Select(m => new ExportMedicinesXMLDto
                        {
                            Name = m.Medicine.Name,
                            Price = m.Medicine.Price,
                            Category = m.Medicine.Category.ToString().ToLower(),
                            ExpiryDate = m.Medicine.ExpiryDate, // Keep it as DateTime
                            Producer = m.Medicine.Producer
                        })
                        .OrderByDescending(m => m.ExpiryDate)
                        .ThenBy(m => m.Price)
                        .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(p => p.FullName)
                .AsEnumerable()
                .Select(p => new ExportPatientsXMLDto
                {
                    FullName = p.FullName,
                    AgeGroup = p.AgeGroup,
                    Gender = p.Gender,
                    Medicines = p.Medicines.Select(m => new ExportMedicinesXMLDto
                    {
                        Name = m.Name,
                        Price = m.Price,
                        Producer = m.Producer,
                        ExpiryDate = m.ExpiryDate, 
                        Category = m.Category,
                    }).ToArray()
                })
                .ToArray();

            foreach (var patient in xml)
            {
                foreach (var medicine in patient.Medicines)
                {
                    medicine.Price = decimal.Parse(medicine.Price.ToString("F2", CultureInfo.InvariantCulture));
                }
            }

            return MyXmlSerializer(xml, "Patients");
        }


        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var json = context.Medicines
                .Where(m => m.Pharmacy.IsNonStop && m.Category == (Category)medicineCategory)
                .AsEnumerable() // Bring data into memory
                .Select(m => new
                {
                    Name = m.Name,
                    Price = decimal.Parse(m.Price.ToString("f2")).ToString(),
                    Pharmacy = new
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber,
                    }

                })
                .OrderBy(m => m.Price)
                .ThenBy(m => m.Name)
                .ToArray();

            return JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
        }

        public static string MyXmlSerializer<T>(T obj, string rootName, bool omitXmlDeclaration = false)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Object to serialize cannot be null.");

            if (string.IsNullOrEmpty(rootName))
                throw new ArgumentNullException(nameof(rootName), "Root name cannot be null or empty.");

            try
            {
                XmlRootAttribute xmlRoot = new(rootName);
                XmlSerializer xmlSerializer = new(typeof(T), xmlRoot);

                XmlSerializerNamespaces namespaces = new();
                namespaces.Add(string.Empty, string.Empty);

                XmlWriterSettings settings = new()
                {
                    OmitXmlDeclaration = omitXmlDeclaration,
                    Indent = true
                };

                StringBuilder sb = new();
                using var stringWriter = new StringWriter(sb);
                using var xmlWriter = XmlWriter.Create(stringWriter, settings);

                xmlSerializer.Serialize(xmlWriter, obj, namespaces);
                return sb.ToString().TrimEnd();
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Serialization error: {ex.Message}");
                throw new InvalidOperationException($"Serializing {typeof(T)} failed.", ex);
            }
        }
    }
}
