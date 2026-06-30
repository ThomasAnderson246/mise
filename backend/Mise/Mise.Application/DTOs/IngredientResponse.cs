using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class IngredientResponse
    {
        public Guid IngredientId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string? Category { get; set; }
        public Guid? DefaultUnitTypeId { get; set; }
        public string? DefaultUnittypeName { get; set; }
        public bool IsNonConvertible { get; set; } = false;
        public List<AllergenTagResponse> Allergens { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
