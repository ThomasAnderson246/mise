using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class CreateMenuItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description {  get; set; } 
        public string? Course { get; set; }
    }
}
