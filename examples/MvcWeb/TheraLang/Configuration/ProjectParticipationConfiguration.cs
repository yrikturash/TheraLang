﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MvcWeb.TheraLang.Entities;

namespace MvcWeb.TheraLang.Configuration
{
    public class ProjectParticipationConfiguration : BaseEntityConfiguration<ProjectParticipation>
    {
        public override void Configure(EntityTypeBuilder<ProjectParticipation> builder)
        {
            builder.ToTable("ProjectParticipations");

            builder.Property(i => i.Status).HasDefaultValue(0);
            builder.Property(y => y.Role).HasDefaultValue(0);
            builder.Property(u => u.ProjectId).IsRequired();
        }
    }
}
