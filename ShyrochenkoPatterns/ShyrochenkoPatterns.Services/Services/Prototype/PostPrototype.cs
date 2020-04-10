using AutoMapper;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Helpers.Static;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Prototype;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Prototype
{
    public class PostPrototype : IPostPrototype
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PostPrototype(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PostResponseModel> CreatePoem(PostRequestModel model)
        {
            var poem = (Poem)StaticPosts.Poem.Clone();

            poem.Synopsis = model.Synopsis;
            poem.Text = model.Text;
            poem.Title = model.Title;
            poem.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository<Poem>().Insert(poem);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(poem);
        }

        public async Task<PostResponseModel> CreateProverb(PostRequestModel model)
        {
            var proverb = (Proverb)StaticPosts.Proverb.Clone();

            if (model.ImageId.HasValue)
                proverb.ImageId = model.ImageId.Value;
            proverb.Text = model.Text;
            proverb.Title = model.Title;
            proverb.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository<Proverb>().Insert(proverb);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(proverb);
        }

        public async Task<PostResponseModel> CreateStory(PostRequestModel model)
        {
            var story = (Story)StaticPosts.Story.Clone();

            if (model.SeriesId.HasValue && model.PartNumber.HasValue)
            {
                story.PartNumber = model.PartNumber.Value;
                story.SeriesId = model.SeriesId.Value;
            }
            story.Text = model.Text;
            story.Title = model.Title;
            story.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository<Story>().Insert(story);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(story);
        }
    }
}
