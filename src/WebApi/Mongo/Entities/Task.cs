using MongoDB.Bson;
using System;

namespace WebApi.Mongo.Entities
{
    public class Task : Entity
    {
        public ObjectId SceneId { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string Body { get; set; }

        public int Thread { get; set; }

        public int Connection { get; set; }

        public int Duration { get; set; }

        public string Unit { get; set; }

        public TaskStatus Status { get; set; }

        public bool IsBaseline { get; set; }

        public DateTime CreationTime { get; set; }
    }

    public enum TaskStatus
    {
        Queue = 0,

        Running = 1,

        Done = 2
    }
}
