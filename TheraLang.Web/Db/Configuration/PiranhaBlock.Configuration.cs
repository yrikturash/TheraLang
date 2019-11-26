﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheraLang.Web.Db.Entities;

namespace TheraLang.Web.Db.Configuration
{
    public class PiranhaBlockConfiguration : IEntityTypeConfiguration<PiranhaBlock>
    {
        public void Configure(EntityTypeBuilder<PiranhaBlock> builder)
        {
            builder.ToTable("Piranha_Blocks");

            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.ClrType)
                .IsRequired()
                .HasColumnName("CLRType")
                .HasMaxLength(256);

            builder.Property(e => e.Title).HasMaxLength(128);
        }
    }
}