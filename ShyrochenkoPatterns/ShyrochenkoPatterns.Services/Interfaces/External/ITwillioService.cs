using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.External
{
    public interface ITwillioService
    {
        Task SendMessageAsync(string to, string body);

        Task<object> Call(string to);
    }
}
