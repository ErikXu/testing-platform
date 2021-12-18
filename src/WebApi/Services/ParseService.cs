using WebApi.Mongo.Entities;

namespace WebApi.Services
{
    public interface IParseService
    {
        public TaskResult ParseStressTestResult(string content);
    }

    public class ParseService : IParseService
    {
        public TaskResult ParseStressTestResult(string content)
        {
            throw new System.NotImplementedException();
        }
    }
}
