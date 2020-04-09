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
            throw new NotImplementedException();
        }

        public async Task<PostResponseModel> CreateStory(PostRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
