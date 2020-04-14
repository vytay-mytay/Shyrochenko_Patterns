using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Services.Interfaces.Proxy;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Proxy
{
    public class Proxy : IProxy
    {
        public Task<MessageResponseModel> SetLike(int id)
        {
            throw new NotImplementedException();
        }
    }
}
