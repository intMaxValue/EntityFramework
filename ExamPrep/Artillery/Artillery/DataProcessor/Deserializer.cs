using System.ComponentModel.DataAnnotations;
using System.Text;
using Artillery.Data.Models;
using Artillery.DataProcessor.ImportDto;
using Newtonsoft.Json;

namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.Data.Models.Enums;
    using System.Data.SqlTypes;
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

            var xml = MyDeserializer<ImportManufacturerXmlDto[]>(xmlString, "Manufacturers");

            var manufacturers = new List<Manufacturer>();

            foreach (var m in xml)
            {
                if (!IsValid(m))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var manufacturer = new Manufacturer();

                manufacturer.ManufacturerName = m.ManufacturerName;
                manufacturer.Founded = m.Founded;
                if (manufacturers.Any(m => m.ManufacturerName == manufacturer.ManufacturerName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                manufacturers.Add(manufacturer);
                sb.AppendLine(string.Format(SuccessfulImportManufacturer, manufacturer.ManufacturerName, manufacturer.Founded));
            }

            context.AddRange(manufacturers);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var xml = MyDeserializer<ImportShellXmlDto[]>(xmlString, "Shells");

            var shells = new List<Shell>();

            foreach (var s in xml)
            {
                if (!IsValid(s) || s.Caliber is null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                var shell = new Shell();

                shell.ShellWeight = s.ShellWeight;
                shell.Caliber = s.Caliber;
                

                shells.Add(shell);
                sb.AppendLine(string.Format(SuccessfulImportShell, shell.Caliber, shell.ShellWeight));
            }

            context.AddRange(shells);
            context.SaveChanges();

            return sb.ToString();


        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var json = JsonConvert.DeserializeObject<ImportGunsJsonDto[]>(jsonString);

            var guns = new List<Gun>();

            foreach (var g in json)
            {
                if (!IsValid(g))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var gun = new Gun();

                if (!Enum.IsDefined(typeof(GunType), g.GunType))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                gun.ManufacturerId = g.ManufacturerId;
                gun.GunWeight = g.GunWeight;
                gun.BarrelLength = g.BarrelLength;
                gun.NumberBuild = g.NumberBuild;
                gun.Range = g.Range;
                gun.GunType = Enum.Parse<GunType>(g.GunType);
                gun.ShellId = g.ShellId;

                var cg = new List<CountryGun>();

                foreach (var c in g.Countries)
                {
                    var countryGun = new CountryGun();

                    countryGun.CountryId = c.Id;
                    cg.Add(countryGun);
                }

                gun.CountriesGuns = cg;

                guns.Add(gun);
                sb.AppendLine(string.Format(SuccessfulImportGun, gun.GunType, gun.GunWeight, gun.BarrelLength));
            }

            context.AddRange(guns);
            context.SaveChanges();

            return sb.ToString();
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