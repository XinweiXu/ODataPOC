using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAPI.Models.Db;

namespace WeatherAPI.Models.EntityConfiguration
{
    /// <summary>
    /// Configures MessageDetails builder
    /// </summary>
    internal class MessageDetailsEntityConfiguration : IEntityTypeConfiguration<MessageDetail>
    {
        public void Configure(EntityTypeBuilder<MessageDetail> builder)
        {
            builder.ToTable("message_details");

            builder.HasIndex(e => e.MessageId, "ix_msgdetails_msgid");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Body).HasColumnName("body");

            builder.Property(e => e.Created)
                .HasColumnName("created")
                .HasDefaultValueSql(Constants.POSTGRES_SQL_UTC_CURRENT_DATETIME);

            builder.Property(e => e.CreatedBy).HasColumnName("created_by");

            builder.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("is_enabled")
                .HasDefaultValueSql("true");

            builder.Property(e => e.LanguageCode)
                .HasMaxLength(10)
                .HasColumnName("language_code");

            builder.Property(e => e.MessageId).HasColumnName("message_id");

            builder.Property(e => e.Modified).HasColumnName("modified");

            builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            builder.Property(e => e.Subject)
                .HasMaxLength(200)
                .HasColumnName("subject");

            builder.HasOne(d => d.Message)
                .WithMany(p => p.MessageDetails)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("fk_msgdetails_msgs_msgid");
        }
    }
}
