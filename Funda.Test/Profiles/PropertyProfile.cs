using AutoMapper;
using Funda.Test.Models;

namespace Funda.Test.Profiles
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile()
        {
            CreateMap<ObjectResult, PropertyListing>();
        }
    }
}
