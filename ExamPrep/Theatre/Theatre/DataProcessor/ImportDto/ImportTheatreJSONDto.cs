using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTheatreJSONDto
    {
        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(1, 10)]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 4)]
        public string Director { get; set; } = null!;

        [Required]
        public ImportTicketsJSONDto[] Tickets { get; set; } = null!;
    }
}
