using System.ComponentModel.DataAnnotations;
using System.Text;
using Artillery.Data.Models;
using Artillery.DataProcessor.ImportDto;

namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var countries = new List<Country>();

            var xml = MyDeserializer<ImportCountriesXmlDto[]>(xmlString, "Countries");

            foreach (var c in xml)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var country = new Country();

                country.CountryName = c.CountryName;
                country.ArmySize = c.ArmySize;

                countries.Add(country);
                sb.AppendLine(string.Format(SuccessfulImportCountry, country.CountryName, country.ArmySize));
            }

            context.AddRange(countries);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            

            return sb.ToString();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            throw new NotImplementedException();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }

        //DESERIALIZER From XML
        private static T MyDeserializer<T>(string inputXml, string rootName)
        {
            try
            {
                XmlRootAttribute root = new XmlRootAttribute(rootName);
                XmlSerializer serializer = new XmlSerializer(typeof(T), root);
                using (StringReader reader = new StringReader(inputXml))
                {
                    T dtos = (T)serializer.Deserialize(reader);
                    return dtos;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error deserializing XML", ex);
            }
        }

    }
}