using System;

namespace WebApi.Mongo.Entities
{
    public class StressScene : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string ContentType { get; set; }

        public string Body { get; set; }

        public int Thread { get; set; }

        public int Connection { get; set; }

        public int Duration { get; set; }

        public string Unit { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
