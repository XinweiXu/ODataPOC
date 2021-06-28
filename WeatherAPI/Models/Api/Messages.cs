using System;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the properties of message list which will be returned by the GET messages end point
    /// </summary>
    public class Messages : Message
    {
        /// <summary>
        /// Gets or sets the information about the engagement totals of the message 
        /// </summary>
        public EngagementTotal EngagementTotal { get; set; }

        /// <summary>
        /// Gets or sets the information about the sent date of the message 
        /// </summary> 
        public DateTime? Sent { get; set; }
    }
}
