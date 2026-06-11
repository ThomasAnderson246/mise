using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Application.Interfaces;
using Mise.Infrastructure.Persistence.Context;

namespace Mise.Infrastructure.Persistence.Repositories
{
    public abstract class BaseTenantRepository<T> : BaseRepository<T>, ITenantRepository<T> where T : class
    {
        protected BaseTenantRepository(MiseDbContext context) : base(context)
        {

        }

        public abstract Task<IEnumerable<T>> GetAllByTenantAsync(Guid tenantId);
        public abstract Task<T?> GetByIdAndTenantAsync(Guid id, Guid tenantId);
        public abstract Task<bool> ExistsInTenantAsync(Guid id, Guid tenantId);
    }
}
