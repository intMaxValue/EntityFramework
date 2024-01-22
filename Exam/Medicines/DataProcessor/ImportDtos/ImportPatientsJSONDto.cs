using Medicines.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Medicines.DataProcessor.ImportDtos
{
    public class ImportPatientsJSONDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string FullName { get; set; } = null!;

        [Required]
        public int AgeGroup { get; set; }

        [Required]
        public int Gender { get; set; }

        public int[] Medicines { get; set; }
    }
}
