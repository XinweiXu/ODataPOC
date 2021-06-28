using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherAPI.Models.Db;
using WeatherAPI.Repository;
using ApiModel = WeatherAPI.Models.Api;

namespace WeatherAPI.Controllers
{
    public class GetMessagesController : ODataController
    {
        private readonly IMessagesRepository _messagesRepository;
        private readonly ILogger _logger;

        public GetMessagesController(IMessagesRepository messagesRepository, ILogger<MessagesController> logger) : base()
        {
            _messagesRepository = messagesRepository;
            _logger = logger;
        }

        [EnableQuery(MaxExpansionDepth=10)]
        [HttpGet("odata/GetMessages({statusId},{languageCode})")]
        public IActionResult GetLimitedMessages([FromODataUri] int statusId, [FromODataUri] string languageCode)
        {
            IQueryable<Message> dbMessages = null;
            try
            {
                // Query the DB using the oData filters and get result in the form of Db Model
                dbMessages = _messagesRepository.FilterEntityList<Message>(statusId, languageCode);

                // Then convert the WaetherAPI.Models.Db.Message to WaetherAPI.Models.Api.Message
                //apiMessages = Convert(dbMessages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting data from DB.");
            }

            return Ok(dbMessages);
        }

        private IQueryable<ApiModel.Message> Convert(IQueryable<Message> dbMessages)
        {
            // Make it working and improved the code using Linq.
            var apiMessages = new List<ApiModel.Message>();
            foreach (var message in dbMessages)
            {
                // Use AutoMapper?
                // Category
                var apiCategory = new ApiModel.MessageCategory
                {
                    CategoryID = message.Category.Id,
                    CategoryDetail = JsonConvert.DeserializeObject<List<ApiModel.CategoryDetail>>(message.Category.Name)
                };

                var apiMessageDetails = new List<ApiModel.MessageDetail>();
                foreach (var messageDetail in message.MessageDetails)
                {
                    var apiMessageDetail = new ApiModel.MessageDetail()
                    {
                        Language = messageDetail.LanguageCode,
                        Subject = messageDetail.Subject,
                        Body = messageDetail.Body
                    };
                    apiMessageDetails.Add(apiMessageDetail);
                }
                var apiMessage = new ApiModel.Message()
                {
                    MessageID = message.Id,
                    Sender = message.SenderId,
                    ExpirationDate = message.Expiry,
                    Important = message.IsImportant,
                    MessageCategory = apiCategory,
                    MessageDetail = apiMessageDetails
                };

                apiMessages.Add(apiMessage);
            }

            return apiMessages.AsQueryable();
        }
    }
}
