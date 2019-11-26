﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TheraLang.Web.TheraLang.DLL.Entities;

namespace TheraLang.Web.TheraLang.DLL.Configuration
{
    public class ResourceCategoriesConfiguration : IEntityTypeConfiguration<ResourceCategory>
    {
        public void Configure(EntityTypeBuilder<ResourceCategory> builder)
        {
            builder.ToTable("ResourceCategories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Type).HasMaxLength(50).IsRequired();

            builder.HasMany(x => x.Resources).WithOne(i => i.ResourceCategory).HasForeignKey(x => x.CategoryId);
        }
    }
}
