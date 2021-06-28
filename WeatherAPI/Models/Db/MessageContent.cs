using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherAPI.Models.Db
{
    public partial class MessageContent
    {
        public long Id { get; set; }
        public Guid ContentId { get; set; }
        public Guid ContentTypeId { get; set; }
        public long MessageId { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual Message Message { get; set; }
    }
}
