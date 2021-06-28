using System;
using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    public class ExpireMessages
    {
        /// <summary>
        /// Gets or sets the array of message ids to exire
        /// </summary>
        [Key]
        [Required]
        public int[] MessageIDs { get; set; }

        /// <summary>
        /// Gets or sets the expiration date to expire messages
        /// </summary>
        public DateTime ExpirationDate { get; set; }
    }
}
