using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Post
{
    public abstract class PostFactory
    {
        public abstract Task<PostResponseModel> Create(PostRequestModel model);
    }
}
