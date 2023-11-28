using Newtonsoft.Json;
using Theatre.DataProcessor.ExportDto;

namespace Theatre.DataProcessor
{

    using System;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres = context.Theatres
                .Where(x => x.NumberOfHalls >= numbersOfHalls && x.Tickets.Count >= 20)
                .ToList()
                .Select(t => new ExportTheatreJSONDto()
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets
                        .Where(ticket => ticket.RowNumber >= 1 && ticket.RowNumber <= 5)
                        .Sum(ticket => ticket.Price),
                    Tickets = t.Tickets
                        .Where(ticket => ticket.RowNumber >= 1 && ticket.RowNumber <= 5)
                        .OrderByDescending(ticket => ticket.Price)
                        .Select(ticket => new ExportTicketsJSONDto()
                        {
                            Price = decimal.Parse(ticket.Price.ToString("f2")),
                            RowNumber = ticket.RowNumber
                        })
                        .ToArray()
                })
                .OrderByDescending(t => t.Halls)
                .ThenBy(t => t.Name)
                .ToList();

            return JsonConvert.SerializeObject(theatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            var plays = context.Plays
                .Where(p => p.Rating <= raiting)
                .AsEnumerable()
                .Select(p => new ExportPlaysXMLDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0  ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),

                    Actors = p.Casts
                        .Where(a => a.IsMainCharacter)
                        .Select(a => new ExportActorsXMLDto()
                        {
                            FullName = a.FullName,
                            MainCharacter = $"Plays main character in '{p.Title}'.",
                        })
                        .OrderByDescending(a => a.FullName)
                        .ToArray()
                })
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .ToArray();

            return MySerializer(plays, "Plays");
        }

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
