using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Domain.Entities
{
    public class TranscriptionJob
    {
        public Guid JobId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? UploadedBy { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public Guid? GeneratedRecipeId { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt = DateTime.UtcNow;
        public DateTime? CompletedAt {  get; set; }

        // navigation
        public Tenant Tenant { get; set; } = null!;
        public User? UploadedByUser { get; set; }
        public Recipe? GeneratedRecipe { get; set; }
        public TranscriptionResult? Result { get; set; }
    }
}
