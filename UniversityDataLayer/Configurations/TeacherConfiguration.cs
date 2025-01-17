﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityDataLayer.Entities;

namespace UniversityDataLayer.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
                     .ValueGeneratedOnAdd();

        builder.HasOne(t => t.Course)
               .WithMany(c => c.Teachers)
               .HasForeignKey(t => t.CourseId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Teachers");
    }
}
