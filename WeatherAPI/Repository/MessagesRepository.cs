using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using WeatherAPI.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace WeatherAPI.Repository
{
    public class MessagesRepository : IMessagesRepository
    {
        protected ILogger Logger { get; set; }
        protected ChangeCommunicationDbContext CCDbContext { get; set; }

        public MessagesRepository(ILogger<MessagesRepository> logger, ChangeCommunicationDbContext ctx)
        {
            Logger = logger;
            CCDbContext = ctx;
        }

        public IQueryable<Message> QueryEntityList<Message>()
        {
            try
            {
                //var result = CCDbContext.Set<TEntity>().AsQueryable(); // Will return all data regardless of odata filter
                var result = (IQueryable<Message>)CCDbContext.Messages
                    .Include(m => m.Status)
                    .Include(m => m.Category)
                    .Include(m => m.MessageDetails); ;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while querying entity list in Web API repository.");
                throw;
            }
        }

        public IQueryable<Message> FilterEntityList<Message>(int statusId, string languageCode)
        {
            try
            {
                //var result = CCDbContext.Set<TEntity>().AsQueryable(); // Will return all data regardless of odata filter
                var result = (IQueryable<Message>)CCDbContext.Messages.Where(m => m.StatusId == statusId && m.MessageDetails.Any(d => d.LanguageCode.Equals(languageCode)))
                    .Include(m => m.Status)
                    .Include(m => m.Category)
                    .Include(m => m.MessageDetails);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while querying entity list in Web API repository.");
                throw;
            }
        }
    }
}
