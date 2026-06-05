using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class TranscriptionResult
    {
        public Guid ResultId { get; set; }
        public Guid JobId { get; set; }
        public string RawResponse { get; set; } = string.Empty;
        public string? ParsedTitle { get; set; }
        public decimal? ConfidenceScore { get; set; }
        public string? FlaggedFields { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //navigation 
        public TranscriptionJob Job { get; set; } = null!;
    }
}
