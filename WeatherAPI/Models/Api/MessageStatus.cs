using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the status of message
    /// </summary>
    /// <remarks>This class is uesd as an EntitySet with OData</remarks>
    public class MessageStatus
    {
        /// <summary>
        /// Gets or sets the ID of the status
        /// </summary>
        public short? StatusID { get; set; }

        /// <summary>
        /// Gets or sets the status name of a message
        /// </summary>
        [Key]
        [Required]
        public string StatusName { get; set; }
    }
}
