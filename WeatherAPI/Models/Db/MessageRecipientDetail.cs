using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Models.Db
{
    public partial class MessageRecipientDetail
    {
        public long Id { get; set; }
        public long RecipientId { get; set; }
        public string PlaceholderValue { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime? Received { get; set; }
        public DateTime? Expiry { get; set; }
        public DateTime? Read { get; set; }
        public DateTime? Acknowledged { get; set; }
        public long MessageDetailId { get; set; }
        public long MessageRecipientId { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual MessageDetail MessageDetail { get; set; }
        public virtual MessageRecipient MessageRecipient { get; set; }
    }
}
