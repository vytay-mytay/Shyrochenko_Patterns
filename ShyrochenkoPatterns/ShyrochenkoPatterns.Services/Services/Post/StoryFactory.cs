using AutoMapper;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Post;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Post
{
    public class StoryFactory : PostFactory
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public StoryFactory(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public override async Task<PostResponseModel> Create(PostRequestModel model)
        {
            var story = _mapper.Map<Story>(model);
            story.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository<Story>().Insert(story);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(story);
        }
    }
}
