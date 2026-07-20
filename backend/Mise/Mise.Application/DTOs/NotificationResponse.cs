using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class NotificationResponse
    {
        public Guid NotificationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid RecipientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type {  get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt {  get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
