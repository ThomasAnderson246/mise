using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class MenuItemAllergenResponse
    {
        public Guid MenuItemAllergenId { get; set; }
        public Guid AllergenId { get; set; }
        public string AllergenName { get; set; } = string.Empty;
        public bool IsMajor { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string? SourceComponent { get; set; }
        public bool IsDirect { get; set; }
        public bool IsManual { get; set; }
    }
}
