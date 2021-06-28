using System.Linq;

namespace WeatherAPI.Repository
{
    public interface IMessagesRepository
    {
        IQueryable<Message> QueryEntityList<Message>();
        IQueryable<Message> FilterEntityList<Message>(int statusId, string languageCode);
    }
}
