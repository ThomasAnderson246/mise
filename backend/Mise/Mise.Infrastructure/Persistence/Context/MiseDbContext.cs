using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mise.Domain.Entities;

namespace Mise.Infrastructure.Persistence.Context
{
	public class MiseDbContext : DbContext
	{

        public MiseDbContext(DbContextOptions<MiseDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MiseDbContext).Assembly);
        }
    }
}
