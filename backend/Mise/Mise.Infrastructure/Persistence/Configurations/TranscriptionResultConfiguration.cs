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
    public class TranscriptionResultConfiguration : IEntityTypeConfiguration<TranscriptionResult>
    {
        public void Configure(EntityTypeBuilder<TranscriptionResult> builder)
        {
            builder.ToTable("transcription_results");

            builder.HasKey(tr => tr.ResultId);

            builder.Property(tr => tr.ResultId)
                .HasColumnName("result_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(tr => tr.JobId)
                .HasColumnName("job_id")
                .IsRequired();

            builder.Property(tr => tr.RawResponse)
                .HasColumnName("raw_response")
                .IsRequired();

            builder.Property(tr => tr.ParsedTitle)
                .HasColumnName("parsed_title")
                .HasMaxLength(255);

            builder.Property(tr => tr.ConfidenceScore)
                .HasColumnName("confidence_score")
                .HasPrecision(4, 3);

            builder.Property(tr => tr.FlaggedFields)
                .HasColumnName("flagged_fields")
                .HasColumnType("jsonb");

            builder.Property(tr => tr.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            builder.HasOne(tr => tr.Job)
                .WithOne(tj => tj.Result)
                .HasForeignKey<TranscriptionResult>(tr => tr.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
