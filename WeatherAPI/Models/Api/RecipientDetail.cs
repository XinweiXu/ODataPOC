using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the recipient of the message
    /// </summary>
    public class RecipientDetail
    {
        /// <summary>
        ///  Gets or sets the Recipient ID
        /// </summary>
        [Required]
        public Guid RecipientID { get; set; }

        /// <summary>
        ///  Gets or sets the type of Recipient which can be User or Group
        /// </summary>
        [Required]
        public short RecipientTypeID { get; set; }
    }
}
