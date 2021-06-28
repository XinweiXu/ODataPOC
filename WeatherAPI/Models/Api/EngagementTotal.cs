namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the number of users who recieved, read or acknowledged a message
    /// </summary>
    public class EngagementTotal
    {
        /// <summary>
        /// Gets or sets the information about number of recipients of the message 
        /// </summary>
        public long Recipients { get; set; }

        /// <summary>
        /// Gets or sets the information about the number of users who read the message 
        /// </summary>
        public long Read { get; set; }

        /// <summary>
        /// Gets or sets the information about the number of users who acknowledged the message 
        /// </summary>
        public long Acknowledged { get; set; }
    }
}
