using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAPI.Models.Db;

namespace WeatherAPI.Models.EntityConfiguration
{
    /// <summary>
    /// Configures MessageRecipient builder
    /// </summary>
    internal class MessageRecipientEntityConfiguration : IEntityTypeConfiguration<MessageRecipient>
    {
        public void Configure(EntityTypeBuilder<MessageRecipient> builder)
        {
            builder.ToTable("message_recipients");

            builder.HasIndex(e => e.MessageId, "ix_msgrcpnts_msgid");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Created)
                .HasColumnName("created")
                .HasDefaultValueSql(Constants.POSTGRES_SQL_UTC_CURRENT_DATETIME);

            builder.Property(e => e.CreatedBy).HasColumnName("created_by");

            builder.Property(e => e.GroupId).HasColumnName("group_id");

            builder.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("is_enabled")
                .HasDefaultValueSql("true");

            builder.Property(e => e.MessageId).HasColumnName("message_id");

            builder.Property(e => e.Modified).HasColumnName("modified");

            builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            builder.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");

            builder.Property(e => e.UserId).HasColumnName("user_id");

            builder.HasOne(d => d.Message)
                .WithMany(p => p.MessageRecipients)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("fk_msgrcpnts_msgs_msgid");
        }
    }
}
