using System.Text;
using Boardgames.Data.Models;
using Boardgames.Data.Models.Enums;
using Boardgames.DataProcessor.ImportDto;
using Newtonsoft.Json;

namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Xml;
    using Boardgames.Data;
   
    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var creators = new List<Creator>();

            var xml = MyXmlDeserializer<ImportCreatorXMLDto[]>(xmlString, "Creators");

            foreach (var c in xml)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var creator = new Creator();
                creator.FirstName = c.FirstName;
                creator.LastName = c.LastName;

                foreach (var bg in c.Boardgames)
                {
                    if (!IsValid(bg))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var boardgame = new Boardgame();
                    boardgame.Name = bg.Name;
                    boardgame.Rating = bg.Rating;
                    boardgame.YearPublished = bg.YearPublished;

                    if (!Enum.IsDefined(typeof(CategoryType), bg.CategoryType))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    boardgame.CategoryType = (CategoryType)bg.CategoryType;
                    boardgame.Mechanics = bg.Mechanics;

                    creator.Boardgames.Add(boardgame);
                }

                creators.Add(creator);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName,
                    creator.Boardgames.Count));
            }

            context.Creators.AddRange(creators);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var sellers = new List<Seller>();

            var json = JsonConvert.DeserializeObject<ImportSellerJSONDto[]>(jsonString);

            var bgIds = context.Boardgames.Select(x => x.Id).ToArray();

            foreach (var s in json)
            {
                if (!IsValid(s))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var seller = new Seller();
                seller.Name = s.Name;
                seller.Address = s.Address;
                seller.Country = s.Country;
                seller.Website = s.Website;

                foreach (var bg in s.Boardgames.Distinct())
                {
                    if (!IsValid(bg) || !bgIds.Contains(bg))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var boardgame = new BoardgameSeller();

                    boardgame.BoardgameId = bg;

                    seller.BoardgamesSellers.Add(boardgame);
                }

                sellers.Add(seller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count));
            }

            context.AddRange(sellers);
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
