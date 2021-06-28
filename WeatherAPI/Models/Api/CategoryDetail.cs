using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the Category name and language code in a particular language.
    /// </summary>
    public class CategoryDetail
    {
        /// <summary>
        /// Gets or sets the category language translation 
        /// </summary>
        [Required]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the category name text
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
