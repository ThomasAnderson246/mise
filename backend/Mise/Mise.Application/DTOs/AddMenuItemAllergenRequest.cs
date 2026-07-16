using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AddMenuItemAllergenRequest
    {
        public Guid AllergenId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string? SourceComponent { get; set; }
    }
}
