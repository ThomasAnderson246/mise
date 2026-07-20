using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class SendDirectMessageRequest
    {
        public Guid RecipientId {  get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
