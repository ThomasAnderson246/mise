using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class TranscriptionRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public string MediaType { get; set; } = "image/jpeg";
    }
}
