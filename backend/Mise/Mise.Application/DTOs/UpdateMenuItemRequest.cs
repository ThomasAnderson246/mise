using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class UpdateMenuItemRequest
    {
        public string? Name {  get; set; }
        public string? Description { get; set; }
        public string? Course { get; set; }
    }
}
