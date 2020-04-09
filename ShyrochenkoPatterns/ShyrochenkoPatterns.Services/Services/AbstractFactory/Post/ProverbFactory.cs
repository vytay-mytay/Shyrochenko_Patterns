using AutoMapper;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Post;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.AbstractFactory.Post
{
    public class ProverbFactory : PostFactory
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ProverbFactory(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public override async Task<PostResponseModel> Create(PostRequestModel model)
        {
            var proverb = _mapper.Map<Proverb>(model);
            proverb.CreationDate = DateTime.UtcNow;

            _unitOfWork.Repository<Proverb>().Insert(proverb);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PostResponseModel>(proverb);
        }
    }
}
