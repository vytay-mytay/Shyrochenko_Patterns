using ShyrochenkoPatterns.Models.Facade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Facade
{
    public interface ICopier
    {
        Task<List<Paper>> Copy(Paper paper, int count);
    }
}
