using System;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the content of the message
    /// </summary>
    public class RelatedContent
    {
        /// <summary>
        /// Gets or sets the ID of the content
        /// </summary>
        public Guid ContentID { get; set; }

        /// <summary>
        /// Gets or sets the type of content 
        /// </summary>
        public Guid ContentTypeID { get; set; }

        /// <summary>
        /// Gets or sets the Url of the content 
        /// </summary>
        public string Url { get; set; }
    }
}
