using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mise.Domain.Entities;

namespace Mise.Application.Interfaces
{
    public interface ITenantRepositoryService : IRepository<Tenant>
    {
        Task<Tenant?> GetBySlugAsync(string slug);
        Task<IEnumerable<Tenant>> GetAllActiveAsync();
        Task<bool> SlugExistsAsync(string slug);
    }
}
