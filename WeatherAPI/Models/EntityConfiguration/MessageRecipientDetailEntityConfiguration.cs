using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAPI.Models.Db;

namespace WeatherAPI.Models.EntityConfiguration
{
    /// <summary>
    /// Configures MessageRecipientDetail builder
    /// </summary>
    internal class MessageRecipientDetailEntityConfiguration : IEntityTypeConfiguration<MessageRecipientDetail>
    {
        public void Configure(EntityTypeBuilder<MessageRecipientDetail> builder)
        {
            builder.ToTable("message_recipient_details");

            builder.HasIndex(e => e.MessageDetailId, "ix_msgrcpntdetails_msgdetailid");

            builder.HasIndex(e => e.MessageRecipientId, "ix_msgrcpntdetails_msgrcpntid");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Acknowledged).HasColumnName("acknowledged");

            builder.Property(e => e.Created)
                .HasColumnName("created")
                .HasDefaultValueSql(Constants.POSTGRES_SQL_UTC_CURRENT_DATETIME);

            builder.Property(e => e.CreatedBy).HasColumnName("created_by");

            builder.Property(e => e.Expiry).HasColumnName("expiry");

            builder.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("is_enabled")
                .HasDefaultValueSql("true");

            builder.Property(e => e.IsExpired).HasColumnName("is_expired");

            builder.Property(e => e.MessageDetailId).HasColumnName("message_detail_id");

            builder.Property(e => e.MessageRecipientId).HasColumnName("message_recipient_id");

            builder.Property(e => e.Modified).HasColumnName("modified");

            builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            builder.Property(e => e.PlaceholderValue)
                .HasColumnType("json")
                .HasColumnName("placeholder_value");

            builder.Property(e => e.Read).HasColumnName("read");

            builder.Property(e => e.Received).HasColumnName("received");

            builder.Property(e => e.RecipientId).HasColumnName("recipient_id");

            builder.HasOne(d => d.MessageDetail)
                .WithMany(p => p.MessageRecipientDetails)
                .HasForeignKey(d => d.MessageDetailId)
                .HasConstraintName("fk_msgrcpntdetails_msgdetails_msgdetailid");

            builder.HasOne(d => d.MessageRecipient)
                .WithMany(p => p.MessageRecipientDetails)
                .HasForeignKey(d => d.MessageRecipientId)
                .HasConstraintName("fk_msgrcpntdetails_msgrcpnts_msgrcpntid");
        }
    }
}
