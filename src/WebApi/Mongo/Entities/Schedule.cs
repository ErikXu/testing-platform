using MongoDB.Bson;
using System;

namespace WebApi.Mongo.Entities
{
    public class Schedule : Entity
    {
        public ObjectId SceneId { get; set; }

        public string SceneName { get; set; }

        public TestType TestType { get; set; }

        public string Cron { get; set; }

        public string Description { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public enum TestType
    {
        Stress = 0,

        Api = 1
    }
}
