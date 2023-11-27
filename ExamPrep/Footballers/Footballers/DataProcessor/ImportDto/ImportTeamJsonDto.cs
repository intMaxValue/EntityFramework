using System.ComponentModel.DataAnnotations;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamJsonDto
    {
        [Required]
        [RegularExpression(@"^[A-Za-z0-9 .-]{3,40}$")]
        public string Name { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Nationality { get; set; }

        [Required]
        public int Trophies { get; set; }

        public int[] Footballers { get; set; }
    }
}
