using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Ewan.Domain.Entities;
using Ewan.Domain.Common;

namespace Ewan.Infrastructure.Persistence
{
    public class EwanDbContext : DbContext
    {
        public EwanDbContext(DbContextOptions<EwanDbContext> options) : base(options) { }

        public DbSet<Banner> Banners => Set<Banner>();
        public DbSet<ServiceItem> ServiceItems => Set<ServiceItem>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<OfferPricingTier> OfferPricingTiers => Set<OfferPricingTier>();
        public DbSet<Faq> Faqs => Set<Faq>();
        public DbSet<Location> Locations => Set<Location>();
        public DbSet<WhyChooseUsFeature> WhyChooseUsFeatures => Set<WhyChooseUsFeature>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
        public DbSet<Inquiry> Inquiries => Set<Inquiry>();
        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EwanDbContext).Assembly);

            // Global Query Filter: أي Entity بترث من BaseEntity، الـ Soft-Deleted بتاعها
            // متترجعش تلقائي في أي Query من غير ما تكتب IgnoreQueryFilters() يدوي
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(EwanDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter),
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                        .MakeGenericMethod(entityType.ClrType);
                    method.Invoke(null, new object[] { modelBuilder });
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : BaseEntity
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}
