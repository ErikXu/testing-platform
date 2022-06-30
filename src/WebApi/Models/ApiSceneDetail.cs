using System.Collections.Generic;
using WebApi.Mongo.Entities;

namespace WebApi.Models
{
    public class ApiSceneDetail : ApiScene
    {
        public List<CollectionItem> CollectionItems { get; set; }

        public bool IsCollectionInvalid { get; set; }

        public EnvironmentInfo EnvironmentInfo { get; set; }

        public bool IsEnvironmentInvalid { get; set; }
    }

    public class CollectionInfo
    {
        public List<CollectionItem> Item { get; set; }
    }

    public class CollectionItem
    {
        public string Name { get; set; }

        public List<CollectionEvent> Event { get; set; }

        public CollectionRequest Request { get; set; }
    }

    public class CollectionRequest
    {
        public string Method { get; set; }

        //public object[] header { get; set; }

        public CollectionUrl Url { get; set; }
    }

    public class CollectionUrl
    {
        public string Raw { get; set; }
    }

    public class CollectionEvent
    {
        public string Listen { get; set; }

        public CollectionScript Script { get; set; }
    }

    public class CollectionScript
    {
        public List<string> Exec { get; set; }

        public string Type { get; set; }
    }

    public class EnvironmentInfo
    {
        public string Name { get; set; }

        public List<EnvironmentValue> Values { get; set; }
    }

    public class EnvironmentValue
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public bool Enabled { get; set; }
    }
}
