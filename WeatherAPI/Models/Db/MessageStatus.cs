using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Models.Db
{
    public partial class MessageStatus
    {
        public MessageStatus()
        {
            Messages = new HashSet<Message>();
        }

        public short Id { get; set; }
        public string StatusName { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
