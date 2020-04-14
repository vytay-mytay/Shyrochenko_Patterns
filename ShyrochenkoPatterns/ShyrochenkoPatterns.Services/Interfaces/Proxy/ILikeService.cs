using ShyrochenkoPatterns.Models.ResponseModels;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Proxy
{
    public interface ILikeService
    {
        Task<MessageResponseModel> SetLike(int id);
    }
}
