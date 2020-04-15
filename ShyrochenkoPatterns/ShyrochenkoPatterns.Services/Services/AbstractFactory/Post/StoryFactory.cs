using AutoMapper;
using Microsoft.AspNetCore.Http;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Post;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.AbstractFactory.Post
{
    public class StoryFactory : PostFactory
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private int? _userId = null;

        public StoryFactory(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

            var context = httpContextAccessor.HttpContext;

            if (context?.User != null)
            {
                try
                {
                    _userId = context.User.GetUserId();
                }
                catch
                {
                    _userId = null;
                }
            }
        }

        public override async Task<PostResponseModel> Create(PostRequestModel model)
        {
            var story = _mapper.Map<Story>(model);
            story.CreationDate = DateTime.UtcNow;
            story.AuthorId = _userId.Value;

            _unitOfWork.Repository<Story>().Insert(story);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(story);
        }
    }
}
