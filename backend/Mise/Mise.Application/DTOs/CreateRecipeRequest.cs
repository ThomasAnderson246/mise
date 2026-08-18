using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class CreateRecipeRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description {  get; set; }
        public string ScalingMode { get; set; } = "multiplier";
        public List<Guid>? CategoryIds { get; set; }
        public bool IsPortion { get; set; } = false;
    }
}
