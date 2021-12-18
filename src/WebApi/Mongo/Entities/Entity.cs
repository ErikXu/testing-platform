using MongoDB.Bson;

namespace WebApi.Mongo.Entities
{
    public abstract class EntityWithTypedId<TId>
    {
        public TId Id { get; set; }
    }

    public abstract class Entity : EntityWithTypedId<ObjectId>
    {

    }
}
