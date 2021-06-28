using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAPI.Models.Db;

namespace WeatherAPI.Models.EntityConfiguration
{
    /// <summary>
    /// Configures MessageContent builder
    /// </summary>
    internal class MessageContentEntityConfiguration : IEntityTypeConfiguration<MessageContent>
    {
        public void Configure(EntityTypeBuilder<MessageContent> builder)
        {
            builder.ToTable("message_contents");

            builder.HasIndex(e => e.MessageId, "ix_msgcontents_msgid");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.ContentId).HasColumnName("content_id");

            builder.Property(e => e.ContentTypeId).HasColumnName("content_type_id");

            builder.Property(e => e.Created)
                .HasColumnName("created")
                .HasDefaultValueSql(Constants.POSTGRES_SQL_UTC_CURRENT_DATETIME);

            builder.Property(e => e.CreatedBy).HasColumnName("created_by");

            builder.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("is_enabled")
                .HasDefaultValueSql("true");

            builder.Property(e => e.MessageId).HasColumnName("message_id");

            builder.Property(e => e.Modified).HasColumnName("modified");

            builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");

            builder.HasOne(d => d.Message)
                .WithMany(p => p.MessageContents)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("fk_msgcontents_msgs_msgid");
        }
    }
}
