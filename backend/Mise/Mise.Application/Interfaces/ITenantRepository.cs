using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mise.Application.Interfaces
{
    public interface ITenantRepository<T> : IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllByTenantAsync(Guid tenantId);
        Task<T?> GetByIdAndTenantAsync(Guid id, Guid tenantid);
        Task<bool> ExistsInTenantAsync(Guid id, Guid tenantid);
    }
}
