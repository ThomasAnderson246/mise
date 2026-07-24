using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.DTOs
{
    public class TranscriptionResponse
    {
        public Guid JobId { get; set; }
        public string Status { get; set; } = string.Empty;
        public TranscriptionResultData? Result { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class TranscriptionResultData
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<TranscribedIngredient> Ingredients { get; set; } = new();
        public List<TranscribedStep> Steps { get; set; } = new();
        public decimal? ConfidenceScore { get; set; }
        public string? RawResponse { get; set; }
    }

    public class TranscribedIngredient
    {
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }

    public class TranscribedStep
    {
        public int StepNumber { get; set; }
        public string Instruction { get; set; } = string.Empty;
        public bool HasTimer { get; set; }
        public int? TimerDuration { get; set; }
    }
}
