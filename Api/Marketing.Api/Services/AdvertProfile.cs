using AutoMapper;
using Marketing.Common.Models;

namespace Marketing.Api.Services
{
    public class AdvertProfile: Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertDbModel, AdvertModel>().ReverseMap();
        }
    }
}