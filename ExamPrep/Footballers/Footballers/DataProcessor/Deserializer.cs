using System.Text;
using Footballers.Data.Models;
using Footballers.Data.Models.Enums;
using Footballers.DataProcessor.ImportDto;
using Newtonsoft.Json;

namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var coaches = new List<Coach>();

            ImportCoachesXmlDto[] xml = MyDeserializer<ImportCoachesXmlDto[]>(xmlString,"Coaches");

            foreach (var c in xml)
            {
                if (!IsValid(c))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var coach = new Coach
                {
                    Name = c.Name,
                    Nationality = c.Nationality,

                };

                

                foreach (var f in c.Footballers)
                {
                    if (!IsValid(f))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var footballer = new Footballer();

                    footballer.Name = f.Name;
                    
                    if (DateTime.TryParseExact(f.ContractStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
                    {
                        footballer.ContractStartDate = startDate;
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (DateTime.TryParseExact(f.ContractEndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
                    {
                        footballer.ContractEndDate = endDate;
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (footballer.ContractStartDate > footballer.ContractEndDate)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (Enum.TryParse(f.BestSkillType.ToString(), out BestSkillType bestSkillType) && Enum.IsDefined(typeof(BestSkillType), bestSkillType))
                    {
                        footballer.BestSkillType = bestSkillType;
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (Enum.TryParse(f.PositionType.ToString(), out PositionType positionType) && Enum.IsDefined(typeof(PositionType), positionType))
                    {
                        footballer.PositionType = positionType;
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    coach.Footballers.Add(footballer);
                }

                coaches.Add(coach);
                sb.AppendLine(string.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));
            }

            context.Coaches.AddRange(coaches);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var teams = new List<Team>();

            var footballersIds = context.Footballers
                .Select(f => f.Id)
                .ToArray();

            var json = JsonConvert.DeserializeObject<ImportTeamJsonDto[]>(jsonString);

            foreach (var t in json)
            {
                var team = new Team();

                if (!IsValid(t) || t.Trophies == 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                team.Name = t.Name;
                team.Nationality = t.Nationality;
                team.Trophies = t.Trophies;

                var footballers = new List<TeamFootballer>();

                foreach (var f in t.Footballers.Distinct())
                {
                    if (!footballersIds.Contains(f))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var footballer = new TeamFootballer();

                    footballer.FootballerId = f;
                    
                    footballers.Add(footballer);
                }

                team.TeamsFootballers = footballers;
                teams.Add(team);
                sb.AppendLine(string.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count()));
            }

            context.AddRange(teams);
            context.SaveChanges();

            return sb.ToString();
        }

        private static bool IsValid(object dto)
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
