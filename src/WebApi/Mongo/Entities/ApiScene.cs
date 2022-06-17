using System;

namespace WebApi.Mongo.Entities
{
    public class ApiScene : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Collection { get; set; }

        public string Environment { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
