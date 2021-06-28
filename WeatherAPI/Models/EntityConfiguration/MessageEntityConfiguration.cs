using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAPI.Models.Db;

namespace WeatherAPI.Models.EntityConfiguration
{
    /// <summary>
    /// Configures Message builder
    /// </summary>
    internal class MessageEntityConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("messages");

            builder.HasIndex(e => e.CategoryId, "ix_msgs_catid");

            builder.HasIndex(e => e.StatusId, "ix_msgs_statid");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Acknowledged).HasColumnName("acknowledged");

            builder.Property(e => e.AllowReminder).HasColumnName("allow_reminder");

            builder.Property(e => e.CategoryId).HasColumnName("category_id");

            builder.Property(e => e.Created)
                .HasColumnName("created")
                .HasDefaultValueSql(Constants.POSTGRES_SQL_UTC_CURRENT_DATETIME);

            builder.Property(e => e.CreatedBy).HasColumnName("created_by");

            builder.Property(e => e.Expiry).HasColumnName("expiry");

            builder.Property(e => e.IsCompleted).HasColumnName("is_completed")
                .HasDefaultValueSql("false");

            builder.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("is_enabled")
                .HasDefaultValueSql("true");

            builder.Property(e => e.IsExpired).HasColumnName("is_expired")
                .HasDefaultValueSql("false");

            builder.Property(e => e.IsImportant).HasColumnName("is_important");

            builder.Property(e => e.Modified).HasColumnName("modified");

            builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            builder.Property(e => e.Read).HasColumnName("read");

            builder.Property(e => e.RequiresAcknowledgement).HasColumnName("requires_acknowledgement");

            builder.Property(e => e.SenderId).HasColumnName("sender_id");

            builder.Property(e => e.Sent).HasColumnName("sent");

            builder.Property(e => e.StatusId).HasColumnName("status_id");

            builder.Property(e => e.TenantId).HasColumnName("tenant_id");

            builder.HasOne(d => d.Category)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("fk_msgs_cats_catid");

            builder.HasOne(d => d.Status)
                .WithMany(p => p.Messages)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("fk_msgs_msgstat_statid");
        }
    }
}
