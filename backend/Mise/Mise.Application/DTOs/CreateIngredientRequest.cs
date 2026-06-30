using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class CreateIngredientRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Category {  get; set; }
        public Guid? DefaultUnitTypeId { get; set; }
        public bool IsNonConvertible { get; set; } = false;
        public List<Guid>? AllergenIds { get; set; }
    }
}
