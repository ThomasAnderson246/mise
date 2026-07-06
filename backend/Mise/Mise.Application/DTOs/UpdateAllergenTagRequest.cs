using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateAllergenTagRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsMajor { get; set; }
    }
}
