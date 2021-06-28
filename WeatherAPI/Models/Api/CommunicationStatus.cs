using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the status of message
    /// </summary>
    /// <remarks>
    /// This class doesn't have any Key associated with it and is used as a ComplexType with OData
    /// </remarks>
    [ComplexType]
    public class CommunicationStatus
    {
        /// <summary>
        /// Gets or sets the ID of the status
        /// </summary>
        public short? StatusID { get; set; }

        /// <summary>
        /// Gets or sets the status name of a message
        /// </summary>
        [Required]
        public string StatusName { get; set; }
    }
}
