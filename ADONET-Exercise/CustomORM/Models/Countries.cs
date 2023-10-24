using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomORM.Minions
{
    public class Countries
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public ICollection<Towns>? Towns { get; set; }
    }
}
