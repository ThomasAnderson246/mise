using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class CreateUnitTypeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Abbreviation { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string MeasureType {  get; set; } = string.Empty;
        public decimal? ConversionFactor { get; set; }
        public bool IsNonConvertible { get; set; } = false;
    }
}
