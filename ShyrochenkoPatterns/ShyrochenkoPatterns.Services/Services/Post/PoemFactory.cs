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
    public class PoemFactory : PostFactory
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public PoemFactory(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public override async Task<PostResponseModel> Create(PostRequestModel model)
        {
            var poem = _mapper.Map<Poem>(model);
            poem.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository<Poem>().Insert(poem);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(poem);
        }
    }
}
