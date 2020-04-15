using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Proxy
{
    public class ReadStoryService : IReadStoryService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        public ReadStoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PostResponseModel> Read(int id)
        {
            var story = await _unitOfWork.Repository<Story>().Get(s => s.Id == id).FirstOrDefaultAsync();

            if (story == null)
                throw new CustomException(HttpStatusCode.BadRequest, "id", "No story with such id");

            return _mapper.Map<PostResponseModel>(story);
        }
    }
}
