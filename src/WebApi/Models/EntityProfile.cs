using AutoMapper;
using WebApi.Mongo.Entities;

namespace WebApi.Models
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            CreateMap<StressSceneCreateForm, StressScene>();
        }
    }
}
