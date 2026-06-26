using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateRecipeRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ScalingMode { get; set; }
        public List<Guid>? CategoryIds { get; set; }
    }
}
