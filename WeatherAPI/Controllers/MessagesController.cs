using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using WeatherAPI.Models.Db;
using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Repository;
using Microsoft.Extensions.Logging;
using ApiModel = WeatherAPI.Models.Api;
using Newtonsoft.Json;

namespace WeatherAPI.Controllers
{
    public class MessagesController : ODataController
    {
        private readonly IMessagesRepository _messagesRepository;
        private readonly ILogger _logger;

        public MessagesController(IMessagesRepository messagesRepository, ILogger<MessagesController> logger) : base()
        {
            _messagesRepository = messagesRepository;
            _logger = logger;
        }

        [EnableQuery]
        [HttpGet("odata/Messages")]
        //public IQueryable<ApiModel.Message> Get()
        public IQueryable<Message> Get()
        {
            IQueryable<Message> dbMessages = null;
            //IQueryable<ApiModel.Message> apiMessages = null;
            try
            {
                // Query the DB using the oData filters and get result in the form of Db Model
                dbMessages = _messagesRepository.QueryEntityList<Message>();

                // Then convert the WaetherAPI.Models.Db.Message to WaetherAPI.Models.Api.Message
                //apiMessages = Convert(dbMessages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred getting data from DB.");
            }

            return dbMessages;
        }

        private IQueryable<ApiModel.Message> Convert(IQueryable<Message> dbMessages)
        {
            // Make it working and improved the code using Linq.
            // The complex type and child table like Messagedetails is currently not loaded.
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
