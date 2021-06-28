using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherAPI.Models.Db;

namespace WeatherAPI.Models.EntityConfiguration
{
    /// <summary>
    /// Configures MessageStatus builder
    /// </summary>
    internal class MessageStatusEntityConfiguration : IEntityTypeConfiguration<MessageStatus>
    {
        public void Configure(EntityTypeBuilder<MessageStatus> builder)
        {
            builder.ToTable("message_status");

            builder.Property(e => e.Id).HasColumnName("id");

            builder.Property(e => e.Created)
                .HasColumnName("created")
                .HasDefaultValueSql(Constants.POSTGRES_SQL_UTC_CURRENT_DATETIME);

            builder.Property(e => e.CreatedBy).HasColumnName("created_by");

            builder.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("status_name");

            builder.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("is_enabled")
                .HasDefaultValueSql("true");

            builder.Property(e => e.Modified).HasColumnName("modified");

            builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        }
    }
}
