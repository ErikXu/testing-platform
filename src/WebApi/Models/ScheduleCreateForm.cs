using WebApi.Mongo.Entities;

namespace WebApi.Models
{
    public class ScheduleCreateForm
    {
        public string SceneId { get; set; }

        public TestType TestType { get; set; }

        /// <summary>
        /// Cron Expression
        /// </summary>
        /// <example>* * * * *</example>
        public string Cron { get; set; }

        public string Description { get; set; }
    }
}
