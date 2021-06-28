using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Api
{
    /// <summary>
    /// Represents information about the properties of a message which will be returned by the POST messages end point
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Message()
        {
            MessageDetail = new List<MessageDetail>();
            RecipientDetail = new List<RecipientDetail>();
            RelatedContent = new List<RelatedContent>();
        }


        /// <summary>
        /// Gets or sets the id of the message
        /// </summary>
        [Key]
        public long MessageID { get; set; }

        /// <summary>
        /// Gets or sets the sender of the message
        /// </summary>
        [Required]
        public Guid Sender { get; set; }

        /// <summary>
        /// Gets or sets the date time of message expiration
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets message subject and body translations
        /// </summary>
        [Required]
        public List<MessageDetail> MessageDetail { get; set; }

        /// <summary>
        /// Gets or sets the information about category of the message 
        /// </summary>
        [Required]
        public MessageCategory MessageCategory { get; set; }

        /// <summary>
        ///  Gets or sets the information about status of the message 
        /// </summary>
        [Required]
        public CommunicationStatus CommunicationStatus { get; set; }

        /// <summary>
        ///  Gets or sets the information about recipients of the message 
        /// </summary>
        [Required]
        public IEnumerable<RecipientDetail> RecipientDetail { get; set; }

        /// <summary>
        ///  Gets or sets the information about the content sent in message
        /// </summary>
        public IEnumerable<RelatedContent> RelatedContent { get; set; }

        /// <summary>
        ///  Gets or sets if the message is flagged important
        /// </summary>
        public bool Important { get; set; }

        /// <summary>
        ///  Gets or sets if the message requires acknowledgement
        /// </summary>
        public bool RequireAcknowledgement { get; set; }

        /// <summary>
        ///  Gets or sets if the message requires expiry on a particular date and time selected on UI
        /// </summary>
        /// </summary>
        public bool RequireExpiry { get; set; }

        /// <summary>
        ///  Gets or sets if the message is allowed to remind tomorrow
        /// </summary>
        public bool AllowReminder { get; set; }
    }
}
