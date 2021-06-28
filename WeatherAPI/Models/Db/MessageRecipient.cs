using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Models.Db
{
    public partial class MessageRecipient
    {
        public MessageRecipient()
        {
            MessageRecipientDetails = new HashSet<MessageRecipientDetail>();
        }

        public long Id { get; set; }
        public Guid[] UserId { get; set; }
        public Guid[] GroupId { get; set; }
        public string Name { get; set; }
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
