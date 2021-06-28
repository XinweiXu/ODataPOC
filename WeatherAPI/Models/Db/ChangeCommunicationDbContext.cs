using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models.EntityConfiguration;

namespace WeatherAPI.Models.Db
{
    public class ChangeCommunicationDbContext : DbContext
    {
        #region Constructor
        /// <summary>
        /// Configures the DbContext class for specific data provider
        /// </summary>
        /// <param name="options">Options to configure our data store</param>
        public ChangeCommunicationDbContext(DbContextOptions<ChangeCommunicationDbContext> options)
            : base(options)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Configures each entity wrt to Database as well as seeds lookup tables
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageContentEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageDetailsEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageRecipientEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageRecipientDetailEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageStatusEntityConfiguration());
        }
        #endregion

        #region Entity DbSet

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Message> Messages { get; set; }

        public virtual DbSet<MessageContent> MessageContents { get; set; }

        public virtual DbSet<MessageDetail> MessageDetails { get; set; }

        public virtual DbSet<MessageRecipient> MessageRecipients { get; set; }

        public virtual DbSet<MessageRecipientDetail> MessageRecipientDetails { get; set; }

        public virtual DbSet<MessageStatus> MessageStatuses { get; set; }

        #endregion
    }
}
