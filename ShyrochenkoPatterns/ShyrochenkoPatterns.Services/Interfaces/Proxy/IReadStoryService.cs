using ShyrochenkoPatterns.Models.ResponseModels.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Proxy
{
    public interface IReadStoryService
    {
        Task<PostResponseModel> Read(int id);
    }
}
