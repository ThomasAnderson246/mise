using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mise.Domain.Entities;

namespace Mise.Infrastructure.Persistence.Configurations
{
    public class TranscriptionJobConfiguration : IEntityTypeConfiguration<TranscriptionJob>
    {
        public void Configure(EntityTypeBuilder<TranscriptionJob> builder)
        {
            builder.ToTable("transcription_jobs");

            builder.HasKey(tj => tj.JobId);

            builder.Property(tj => tj.JobId)
                .HasColumnName("job_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(tj => tj.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(tj => tj.UploadedBy)
                .HasColumnName("uploaded_by");

            builder.Property(tj => tj.ImageUrl)
                .HasColumnName("image_url")
                .IsRequired();

            builder.Property(tj => tj.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(tj => tj.GeneratedRecipeId)
                .HasColumnName("generated_recipe_id");

            builder.Property(tj => tj.ErrorMessage)
                .HasColumnName("error_message");

            builder.Property(tj => tj.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.Property(tj => tj.CompletedAt)
                .HasColumnName("completed_at");

            builder.HasOne(tj => tj.Tenant)
                .WithMany()
                .HasForeignKey(tj => tj.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tj => tj.UploadedByUser)
                .WithMany()
                .HasForeignKey(tj => tj.UploadedBy)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(tj => tj.GeneratedRecipe)
                .WithMany()
                .HasForeignKey(tj => tj.GeneratedRecipeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
