using System;
using System.Collections.Generic;

namespace WeatherAPI.Models.Db
{
    public partial class MessageDetail
    {
        public MessageDetail()
        {
            MessageRecipientDetails = new HashSet<MessageRecipientDetail>();
        }

        public long Id { get; set; }
        public string LanguageCode { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public long MessageId { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Message Message { get; set; }
        public virtual ICollection<MessageRecipientDetail> MessageRecipientDetails { get; set; }
    }
}
