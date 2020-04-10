using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.Domain.Entities;
using ShyrochenkoPatterns.Domain.Entities.Chat;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Chat;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using Profile = ShyrochenkoPatterns.Domain.Entities.Identity.Profile;

namespace ShyrochenkoPatterns.Services.StartApp
{
    public class AutoMapperProfileConfiguration : AutoMapper.Profile
    {
        public AutoMapperProfileConfiguration()
        : this("MyProfile")
        {
        }

        protected AutoMapperProfileConfiguration(string profileName)
        : base(profileName)
        {
            CreateMap<UserProfileRequestModel, Profile>()
                .ForMember(t => t.Id, opt => opt.Ignore())
                .ForMember(t => t.User, opt => opt.Ignore())
                .ForMember(t => t.UserId, opt => opt.Ignore());

            CreateMap<Profile, UserProfileResponseModel>()
                .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Avatar))
                .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User != null ? x.User.Email : ""))
                .ForMember(t => t.PhoneNumber, opt => opt.MapFrom(x => x.User != null ? x.User.PhoneNumber : ""))
                .ForMember(t => t.IsBlocked, opt => opt.MapFrom(x => x.User != null ? !x.User.IsActive : false));

            CreateMap<Image, ImageResponseModel>();

            CreateMap<UserDevice, UserDeviceResponseModel>()
                .ForMember(t => t.AddedAt, opt => opt.MapFrom(src => src.AddedAt.ToISO()));

            CreateMap<Message, ChatMessageBaseResponseModel>()
                .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
                .ForMember(t => t.Image, opt => opt.MapFrom(x => x.Image));

            CreateMap<Message, ChatMessageResponseModel>()
                .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
                .ForMember(t => t.Image, opt => opt.MapFrom(x => x.Image));

            CreateMap<PostRequestModel, Poem>();

            CreateMap<PostRequestModel, Proverb>();

            CreateMap<PostRequestModel, Story>();

            CreateMap<Story, PostResponseModel>()
                .ForMember(t => t.Type, opt => opt.MapFrom(x => PostType.Story));

            CreateMap<Poem, PostResponseModel>()
                .ForMember(t => t.Type, opt => opt.MapFrom(x => PostType.Poem));

            CreateMap<Proverb, PostResponseModel>()
                .ForMember(t => t.Type, opt => opt.MapFrom(x => PostType.Proverb));
        }
    }
}
