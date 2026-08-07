using Ewan.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Infrastructure.Configurations
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.Property(o => o.TitleAr).HasMaxLength(300).IsRequired();
            builder.Property(o => o.TitleEn).HasMaxLength(300).IsRequired();
            builder.Property(o => o.ImageUrl).HasMaxLength(500).IsRequired();

            builder.HasMany(o => o.PricingTiers)
                .WithOne(t => t.Offer)
                .HasForeignKey(t => t.OfferId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
