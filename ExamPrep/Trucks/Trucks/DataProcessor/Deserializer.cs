using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Trucks.Data.Models;
using Trucks.Data.Models.Enums;
using Trucks.DataProcessor.ImportDto;

namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;
    using Data;


    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            var despatchers = new List<Despatcher>();

            var xml = MyDeserializer<ImportDespatcherDto[]>(xmlString, "Despatchers");
            
            var sb = new StringBuilder();
            
            foreach (var d in xml)
            {
                Despatcher despatcher = new Despatcher();


                if (!IsValid(d) || string.IsNullOrWhiteSpace(d.Name) || string.IsNullOrWhiteSpace(d.Position))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                despatcher.Name = d.Name;
                despatcher.Position = d.Position;

                foreach (var t in d.Trucks)
                {
                    var truck = new Truck();

                    if (!IsValid(t))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    truck.RegistrationNumber = t.RegistrationNumber;
                    truck.VinNumber = t.VinNumber;
                    truck.TankCapacity = t.TankCapacity;
                    truck.CargoCapacity = t.CargoCapacity;
                    
                    if (Enum.TryParse<CategoryType>(t.CategoryType.ToString(), out var categoryType))
                    {
                        truck.CategoryType = categoryType;
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (Enum.TryParse<MakeType>(t.MakeType.ToString(), out var makeType))
                    {
                        truck.MakeType = makeType;
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    
                    despatcher.Trucks.Add(truck);
                }

                despatchers.Add(despatcher);
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, despatcher.Trucks.Count));
            }

            context.Despatchers.AddRange(despatchers);
            context.SaveChanges();
            
            return sb.ToString();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            var sb = new StringBuilder();
            var clients = new List<Client>();

            var json = JsonConvert.DeserializeObject<ImportClientsDto[]>(jsonString);

            foreach (var c in json)
            {
                if (!IsValid(c) || c.Type == "usual" || string.IsNullOrWhiteSpace(c.Nationality))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var client = new Client
                {
                    Name = c.Name,
                    Nationality = c.Nationality,
                    Type = c.Type,
                };

                foreach (var tr in c.Trucks.Distinct())
                {
                    var existingTruck = context.Trucks.FirstOrDefault(t => t.Id == tr);

                    if (existingTruck == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var clientTruck = new ClientTruck
                    {
                        TruckId = existingTruck.Id,
                    };

                    client.ClientsTrucks.Add(clientTruck);
                }

                clients.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count));
            }

            context.Clients.AddRange(clients);
            context.SaveChanges();

            return sb.ToString();
        }




        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

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