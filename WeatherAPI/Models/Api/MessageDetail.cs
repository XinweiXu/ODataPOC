using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the language translations of a subject and body
    /// </summary>
    public class MessageDetail
    {
        /// <summary>
        /// Gets or sets the Language
        /// </summary>
        [Required]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [Required]
        public string Subject { get; set; }
        /// <summary>
        /// Gets or sets the body
        /// </summary>
        [Required]
        public string Body { get; set; }
    }
}
