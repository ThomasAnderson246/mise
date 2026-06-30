using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class AllergenTagResponse
    {
        public Guid AllergenId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public bool IsMajor { get; set; } = false;
    }
}
