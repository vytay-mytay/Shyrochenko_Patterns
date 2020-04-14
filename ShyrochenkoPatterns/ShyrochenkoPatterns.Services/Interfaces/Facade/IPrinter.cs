using ShyrochenkoPatterns.Models.Facade;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Facade
{
    public interface IPrinter
    {
        public Task<Paper> Print(byte[] array);
    }
}
