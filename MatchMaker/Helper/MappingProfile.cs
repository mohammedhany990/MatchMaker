using AutoMapper;
using MatchMaker.Core.DTOs;
using MatchMaker.Core.Entities;

namespace MatchMaker.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url));

            CreateMap<Photo, PhotoDto>();

            CreateMap<RegisterCommand, AppUser>();

            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));

            CreateMap<Message, MessageDto>()
                .ForMember(dest => dest.SenderPhotoUrl,
                    opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url))
                .ForMember(dest => dest.RecipientPhotoUrl,
                    opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain)!.Url));


            CreateMap<DateTime, DateTime>()
                .ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

            CreateMap<DateTime?, DateTime?>()
                .ConvertUsing(d => d.HasValue ?
                    DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);


        }
    }
}
