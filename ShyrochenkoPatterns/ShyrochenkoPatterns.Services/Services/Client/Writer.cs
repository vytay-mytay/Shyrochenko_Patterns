using ShyrochenkoPatterns.Models.RequestModels.Posts;
using ShyrochenkoPatterns.Models.ResponseModels.Post;
using ShyrochenkoPatterns.Services.Interfaces.Post;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Client
{
    public class Writer
    {
        private PostFactory _factory;

        public Writer(PostFactory factory)
        {
            _factory = factory;
        }

        public async Task<PostResponseModel> Create(PostRequestModel model)
        {
            return await _factory.Create(model);
        }
    }
}
