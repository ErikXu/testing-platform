using MongoDB.Bson;

namespace WebApi.Mongo.Entities
{
    public class SceneAgentMap : Entity
    {
        public ObjectId SceneId { get; set; }

        public ObjectId AgentId { get; set; }
    }
}
