using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the category
    /// </summary>
    /// <remarks>This class is uesd as an EntitySet with OData</remarks>
    public class Category
    {
        /// <summary>
        ///  Gets or sets the ID of the category
        /// </summary>
        [Key]
        [Required]
        public short CategoryID { get; set; }

        /// <summary>
        ///  Gets or sets the translations of the category 
        /// </summary>
        public List<CategoryDetail> CategoryDetail { get; set; }
    }
}
