﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MvcWeb.TheraLang.Entities;

namespace MvcWeb.TheraLang.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Name).HasMaxLength(250).IsRequired();
            builder.Property(e => e.Type).HasMaxLength(250).IsRequired();

            builder.HasMany(i => i.ResourceProjects).WithOne(e => e.Project).HasForeignKey(p=>p.ProjectId);
            builder.HasMany(i => i.ProjectParticipations).WithOne(t => t.Project).HasForeignKey(p => p.ProjectId);
        }
    }
}
