using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the category
    /// </summary>
    /// <remarks>
    /// This class doesn't have any Key associated with it and is used as a ComplexType with OData
    /// </remarks>
    [ComplexType]
    public class MessageCategory
    {
        /// <summary>
        ///  Gets or sets the ID of the category
        /// </summary>
        [Required]
        public long CategoryID { get; set; }

        /// <summary>
        ///  Gets or sets the translations of the category 
        /// </summary>
        public List<CategoryDetail> CategoryDetail { get; set; }
    }
}
