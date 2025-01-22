using Backend.Infrastructure.Database;

namespace Backend.Features.Logs
{
    public interface ILogService
    {
        void LogRequest(string apiKey, string requestType, string responseType, string? requestBody, string? actionName);
    }

    public class LogService : ILogService
    {
        private readonly CargoHubDbContext _dbContext;

        public LogService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void LogRequest(string apiKey, string requestType, string? responseType, string? requestBody, string? actionName)
        {
            var log = new Log
            {
                ApiKey = apiKey,
                Timestamp = DateTime.UtcNow,
                RequestType = requestType,
                ResponeType = responseType,
                RequestBody = requestBody,
                ActionName = actionName
            };

            _dbContext.Logs?.Add(log);
            _dbContext.SaveChanges();
        }
    }
}
