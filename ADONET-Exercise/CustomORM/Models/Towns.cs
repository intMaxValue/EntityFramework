using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomORM.Minions
{
    public class Towns 
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        [ForeignKey(nameof(Country))]
        public int? CountryCode { get; set; }

        public Countries? Country { get; set; }
    }
}
