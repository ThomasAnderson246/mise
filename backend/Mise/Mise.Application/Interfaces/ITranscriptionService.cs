using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Application.DTOs;

namespace Mise.Application.Interfaces
{
    public interface ITranscriptionService
    {
        Task<TranscriptionResponse> TranscribeAsync( TranscriptionRequest request, Guid tenantId, Guid uploadedBy);
        Task<TranscriptionResponse?> GetJobAsync(Guid jobId, Guid tenantId);
        Task<IEnumerable<TranscriptionResponse>> GetJobsByTenantAsync(Guid tenantId);
        
    }
}
