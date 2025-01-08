using Backend.Infrastructure.Database;

namespace Backend.Features.Logs
{
    public interface ILogService
    {
        void LogRequest(string apiKey, string requestType, string responseType, string? requestBody);
    }
}

namespace Backend.Features.Logs
{
    public class LogService : ILogService
    {
        private readonly CargoHubDbContext _dbContext;

        public LogService(CargoHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void LogRequest(string apiKey, string requestType, string? responseType, string? requestBody)
        {
            var log = new Log
            {
                ApiKey = apiKey,
                Timestamp = DateTime.UtcNow,
                RequestType = requestType,
                ResponeType = responseType,
                RequestBody = requestBody
            };

            _dbContext.Logs?.Add(log);
            _dbContext.SaveChanges();
        }
    }
}
