using Ewan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Infrastructure.Configurations
{
    public class OfferPricingTierConfiguration : IEntityTypeConfiguration<OfferPricingTier>
    {
        public void Configure(EntityTypeBuilder<OfferPricingTier> builder)
        {
            builder.Property(t => t.NationalityAr).HasMaxLength(200).IsRequired();
            builder.Property(t => t.NationalityEn).HasMaxLength(200).IsRequired();
            builder.Property(t => t.DurationAr).HasMaxLength(100).IsRequired();
            builder.Property(t => t.DurationEn).HasMaxLength(100).IsRequired();
            builder.Property(t => t.Price).HasPrecision(18, 2);
        }
    }
}
