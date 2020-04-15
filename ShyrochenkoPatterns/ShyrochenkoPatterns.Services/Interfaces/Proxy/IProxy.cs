using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Proxy
{
    public interface IProxy : IReadStoryService
    {
        Task<PostResponseModel> Create(PostRequestModel model);

        Task SetLike(int id);
    }
}
