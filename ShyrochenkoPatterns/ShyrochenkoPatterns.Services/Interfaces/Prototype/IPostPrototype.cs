using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Prototype
{
    public interface IPostPrototype
    {
        public Task<PostResponseModel> CreatePoem(PostRequestModel model);

        public Task<PostResponseModel> CreateStory(PostRequestModel model);

        public Task<PostResponseModel> CreateProverb(PostRequestModel model);
    }
}
