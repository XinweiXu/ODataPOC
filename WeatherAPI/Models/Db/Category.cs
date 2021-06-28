using System;
using System.Collections.Generic;

namespace WeatherAPI.Models.Db
{
    public partial class Category
    {
        public Category()
        {
            Messages = new HashSet<Message>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
