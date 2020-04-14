using ShyrochenkoPatterns.Models.Facade;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Facade
{
    public interface IScaner
    {
        public Task<byte[]> Scan(Paper paper);
    }
}
