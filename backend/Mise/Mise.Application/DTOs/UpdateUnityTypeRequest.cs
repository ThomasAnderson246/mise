using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateUnityTypeRequest
    {
        public string? Name { get; set; }
        public string? Abbreviation { get; set; }
        public decimal? ConversionFactor { get; set; }
        public bool? IsNonConvertible {  get; set; }
    }
}
