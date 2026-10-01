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
    public class PriorityItemConfiguration
    {

        public void Configure(EntityTypeBuilder<PriorityItem> builder)
        {
            builder.ToTable("priority_items");

            builder.HasKey(p => p.PriorityItemId);

            builder.Property(p => p.PriorityItemId)
                .HasColumnName("priority_item_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.TenantId)
                .HasColumnName("tenant_id")
                .IsRequired();

            builder.Property(p => p.SourceType)
                .HasColumnName("source_type")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.RecipeId)
                .HasColumnName("recipe_id");

            builder.Property(p => p.ItemName)
                .HasColumnName("item_name");

            builder.Property(p => p.ScalingFactor)
                .HasColumnName("scaling_factor")
                .HasPrecision(10, 4);

            builder.Property(p => p.AnchorIngredientId)
                .HasColumnName("anchor_ingredient_id");

            builder.Property(p => p.AnchorQuantity)
                .HasColumnName("anchor_quantity")
                .HasPrecision(10, 4);

            builder.Property(p => p.Notes)
                .HasColumnName("notes");

            builder.Property(p => p.Origin)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(p => p.ReasonCode)
                .HasColumnName("reason_code")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(p => p.ReasonNote)
                .HasColumnName("reason_note");

            builder.Property(p => p.FlaggedBy)
                .HasColumnName("flagged_by")
                .IsRequired();

            builder.Property(p => p.SourcePrepListId)
                .HasColumnName("source_prep_list_id");

            builder.Property(p => p.SourcePrepListItemId)
                .HasColumnName("source_prep_list_item_id");

            builder.Property(p => p.CreatedBy)
                .HasColumnName("created_by")
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            //keys & restraints

            builder.HasOne(p => p.Tenant)
                .WithMany()
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Recipe)
                .WithMany()
                .HasForeignKey(p => p.RecipeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.AnchorIngredient)
                .WithMany()
                .HasForeignKey(p => p.AnchorIngredientId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.FlaggedByUser)
                .WithMany()
                .HasForeignKey(p => p.FlaggedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.CreateDByUser)
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // set null, no tcascade: must survive a list being dleted later. 
            // it is an independant record, not a child of it
            builder.HasOne(p => p.SourcePrepList)
                .WithMany()
                .HasForeignKey(p => p.SourcePrepListId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.SourcePrepListItem)
                .WithMany()
                .HasForeignKey(p => p.SourcePrepListItemId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
