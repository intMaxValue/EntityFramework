using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Theatre.Data.Models;
using Theatre.DataProcessor.ImportDto;

namespace Theatre.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using Theatre.Data;
    using Theatre.Data.Models.Enums;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";



        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();

            List<Play> plays = new List<Play>();

            var xml = MyDeserializer<ImportPlaysXMLDto[]>(xmlString, "Plays");

            foreach (var p in xml)
            {
                if (!IsValid(p) || string.IsNullOrWhiteSpace(p.Title) || string.IsNullOrWhiteSpace(p.Description) || string.IsNullOrWhiteSpace(p.Screenwriter))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var play = new Play();

                play.Title = p.Title;

                if (TimeSpan.TryParseExact(p.Duration, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out var duration))
                {
                    if (duration < TimeSpan.FromHours(1))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    play.Duration = duration;
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                play.Rating = p.Rating;

                if (Enum.TryParse<Genre>(p.Genre, out var genre))
                {
                    play.Genre = genre;
                }
                else
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                play.Description = p.Description;
                play.Screenwriter = p.Screenwriter;

                

                plays.Add(play);
                sb.AppendLine(string.Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating));
            }

            context.AddRange(plays);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            var sb = new StringBuilder();

            List<Cast> casts = new List<Cast>();

            var xml = MyDeserializer<ImportCastsXMLDto[]>(xmlString, "Casts");

            foreach (var c in xml)
            {
                if (!IsValid(c) || string.IsNullOrWhiteSpace(c.FullName) || string.IsNullOrWhiteSpace(c.PhoneNumber))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var cast = new Cast();

                cast.FullName = c.FullName;
                cast.IsMainCharacter = c.IsMainCharacter;
                cast.PhoneNumber = c.PhoneNumber;
                cast.PlayId = c.PlayId;


                casts.Add(cast);
                sb.AppendLine(string.Format(SuccessfulImportActor, cast.FullName, cast.IsMainCharacter == false ? "lesser" : "main"));
            }

            context.AddRange(casts);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var theatres = new List<Data.Models.Theatre>();

            var json = JsonConvert.DeserializeObject<ImportTheatreJSONDto[]>(jsonString);

            foreach (var t in json)
            {
                if (!IsValid(t) || string.IsNullOrWhiteSpace(t.Name) || string.IsNullOrWhiteSpace(t.Director) || t.NumberOfHalls == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var theatre = new Data.Models.Theatre();

                theatre.Name = t.Name;
                theatre.Director = t.Director;
                theatre.NumberOfHalls = t.NumberOfHalls;

                foreach (var tck in t.Tickets)
                {
                    if (!IsValid(tck) || tck.PlayId <= 0 || tck.Price <= 0 || tck.RowNumber < 0)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var ticket = new Ticket();
                    ticket.Price = tck.Price;
                    ticket.RowNumber = tck.RowNumber;
                    ticket.PlayId = tck.PlayId;

                    theatre.Tickets.Add(ticket);
                }

                theatres.Add(theatre);
                sb.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name, theatre.Tickets.Count));
            }

            context.AddRange(theatres);
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
