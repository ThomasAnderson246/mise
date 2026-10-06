using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class CompletePrepListRequest
    {
        public List<UntouchedItemReason> UntouchedItemReasons { get; set; } = new();
    }

    public class UntouchedItemReason
    {
        public Guid PrepListItemId { get; set; }
        public string ReasonCode { get; set; } = string.Empty;
        public string? ReasonNote { get; set; }
    }
}
