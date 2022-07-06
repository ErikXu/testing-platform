namespace WebApi.Mongo.Entities
{
    public class CallbackSetting : Entity
    {
        public bool IsStressTestEnabled { get; set; }

        public bool IsApiTestEnabled { get; set; }
    }
}
