using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Domain.Entities.Post;
using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;
using ShyrochenkoPatterns.Services.Services.AbstractFactory.Client;
using ShyrochenkoPatterns.Services.Services.AbstractFactory.Post;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Proxy
{
    public class Proxy : IProxy
    {
        private static List<PostResponseModel> Stories = new List<PostResponseModel>();

        private IReadStoryService _readStory;
        private Writer _story;
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;

        private int? _userId = null;

        public Proxy(IReadStoryService readStory, IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _readStory = readStory;
            _story = new Writer(new StoryFactory(unitOfWork, mapper, httpContextAccessor));

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

        public async Task<PostResponseModel> Read(int id)
        {
            var response = Stories.FirstOrDefault(s => s.Id == id);

            if (response == null)
                response = await _readStory.Read(id);

            return response;
        }

        public async Task<PostResponseModel> Create(PostRequestModel model)
        {
            var currentPopilarity = await _unitOfWork.Repository<ApplicationUser>()
                                     .Get(u => u.Id == _userId)
                                     .Select(u => u.Popularity)
                                     .FirstOrDefaultAsync();

            var lessPopularUser = await _unitOfWork.Repository<ApplicationUser>()
                                    .Get(u => Stories.Select(s => s.AuthorId).Contains(u.Id) && u.Popularity < currentPopilarity || u.Popularity <= currentPopilarity)
                                    .FirstOrDefaultAsync();

            PostResponseModel response = null;

            response = await _story.Create(model);

            if (Stories.Count < 10)
                Stories.Add(response);
            else if (lessPopularUser != null)
                Stories.Replace(Stories.OrderBy(s => s.CreationDate).FirstOrDefault(s => s.AuthorId == lessPopularUser.Id), response);

            return response;
        }

        public async Task SetLike(int id)
        {
            var story = await _unitOfWork.Repository<Story>().Get(s => s.Id == id).Include(s => s.Author).FirstOrDefaultAsync();

            if (story == null)
                throw new CustomException(HttpStatusCode.BadRequest, "id", "No story with such id");

            story.LikeCount++;

            _unitOfWork.Repository<Story>().Update(story);
            await _unitOfWork.SaveChangesAsync();

            var avarageAuthorLikes = await _unitOfWork.Repository<Story>()
                                .Get(s => s.AuthorId == story.AuthorId)
                                .Select(s => s.LikeCount)
                                .AverageAsync();
            var usersCount = await _unitOfWork.Repository<ApplicationUser>().Table.CountAsync();

            story.Author.Popularity = (avarageAuthorLikes / usersCount) * 100;
            _unitOfWork.Repository<ApplicationUser>().Update(story.Author);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
