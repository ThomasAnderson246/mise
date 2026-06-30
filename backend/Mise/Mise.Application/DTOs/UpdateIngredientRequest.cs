using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateIngredientRequest
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public Guid? DefaultUnitTypeId { get; set; }
        public bool? IsNonConvertible { get; set; }
        public List<Guid>? AllergenIds { get; set; }

    }
}
