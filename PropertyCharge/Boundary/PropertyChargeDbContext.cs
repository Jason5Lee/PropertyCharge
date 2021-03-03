using Jason5Lee.PropertyCharge.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jason5Lee.PropertyCharge.Boundary
{
    public class PropertyChargeDbContext : DbContext
    {
        public PropertyChargeDbContext(DbContextOptions<PropertyChargeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Personale> Personales { get; set; }
        public DbSet<Charge> Charges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Charge>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
