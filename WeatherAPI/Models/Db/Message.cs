using System;
using System.Collections.Generic;

namespace WeatherAPI.Models.Db
{
    public partial class Message
    {
        public Message()
        {
            MessageContents = new HashSet<MessageContent>();
            MessageDetails = new HashSet<MessageDetail>();
            MessageRecipients = new HashSet<MessageRecipient>();
        }

        public long Id { get; set; }
        public bool? AllowReminder { get; set; }
        public Guid? TenantId { get; set; }
        public Guid SenderId { get; set; }
        public short CategoryId { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsImportant { get; set; }
        public bool IsExpired { get; set; }
        public bool RequiresAcknowledgement { get; set; }
        public short StatusId { get; set; }
        public DateTime? Expiry { get; set; }
        public DateTime? Read { get; set; }
        public DateTime? Acknowledged { get; set; }
        public DateTime? Sent { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Category Category { get; set; }
        public virtual MessageStatus Status { get; set; }
        public virtual ICollection<MessageContent> MessageContents { get; set; }
        public virtual ICollection<MessageDetail> MessageDetails { get; set; }
        public virtual ICollection<MessageRecipient> MessageRecipients { get; set; }
    }
}
