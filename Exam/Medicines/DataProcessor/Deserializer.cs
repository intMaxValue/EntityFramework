using System.Text;
using Medicines.Data.Models;
using Medicines.Data.Models.Enums;
using Medicines.DataProcessor.ImportDtos;
using Newtonsoft.Json;

namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Xml;
    using System.Globalization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var patients = new List<Patient>();

            var json = JsonConvert.DeserializeObject<ImportPatientsJSONDto[]>(jsonString);

            foreach (var p in json)
            {
                if (!IsValid(p))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var patient = new Patient();

                patient.FullName = p.FullName;

                if (!Enum.IsDefined(typeof(AgeGroup), p.AgeGroup) || p.AgeGroup < 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                patient.AgeGroup = (AgeGroup)p.AgeGroup;

                if (!Enum.IsDefined(typeof(Gender), p.Gender) || p.Gender < 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                patient.Gender = (Gender)p.Gender;


                foreach (var m in p.Medicines)
                {
                    if (patient.PatientsMedicines.Any(pm => pm.MedicineId == m))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var medicine = new PatientMedicine();
                    medicine.MedicineId = m;
                    patient.PatientsMedicines.Add(medicine);
                }

                patients.Add(patient);
                sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName,
                    patient.PatientsMedicines.Count));
            }

            context.AddRange(patients);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var pharmacies = new List<Pharmacy>();

            var xml = MyXmlDeserializer<ImportPharmaciesXMLDto[]>(xmlString, "Pharmacies");

            foreach (var p in xml)
            {
                if (!IsValid(p))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var pharmacy = new Pharmacy();
                pharmacy.Name = p.Name;
                pharmacy.PhoneNumber = p.PhoneNumber;

                if (p.IsNonStop != "true" && p.IsNonStop != "false" )
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                pharmacy.IsNonStop = bool.Parse(p.IsNonStop);

                foreach (var m in p.Medicines)
                {
                    if (!IsValid(m))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var medicine = new Medicine();

                    medicine.Name = m.Name;
                    medicine.Price = m.Price;

                    if (!Enum.IsDefined(typeof(Category), m.Category))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    medicine.Category = (Category)m.Category;


                    var prodDate = DateTime.ParseExact(m.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    
                        if (prodDate == null)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                    medicine.ProductionDate = prodDate;

                    var expDate = DateTime.ParseExact(m.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    if (expDate == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    medicine.ExpiryDate = expDate;

                    if (medicine.ProductionDate >= medicine.ExpiryDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(m.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    medicine.Producer = m.Producer;

                    if (pharmacy.Medicines.Any(p => p.Name == m.Name && p.Producer == m.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    pharmacy.Medicines.Add(medicine);
                }

                pharmacies.Add(pharmacy);
                sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count));
            }

            context.AddRange(pharmacies);
            context.SaveChanges();

            return sb.ToString();
        }


        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

        public static T MyXmlDeserializer<T>(string inputXml, string rootName)
        {
            if (string.IsNullOrEmpty(inputXml))
                throw new ArgumentException("Input XML cannot be null or empty.", nameof(inputXml));

            if (string.IsNullOrEmpty(rootName))
                throw new ArgumentException("Root name cannot be null or empty.", nameof(rootName));

            try
            {
                XmlRootAttribute xmlRoot = new(rootName);
                XmlSerializer xmlSerializer = new(typeof(T), xmlRoot);

                using var reader = new StringReader(inputXml);
                return (T)xmlSerializer.Deserialize(reader);
            }
            catch (XmlException ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException("XML deserialization failed.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine(ex);
                throw new InvalidOperationException($"{typeof(T)} deserialization failed.", ex);
            }
        }
    }
}
