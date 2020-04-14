using ShyrochenkoPatterns.Models.Facade;
using ShyrochenkoPatterns.Services.Interfaces.Facade;
using System.Text;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Facade
{
    public class Scaner : IScaner
    {
        public async Task<byte[]> Scan(Paper paper)
        {
            var response = Encoding.Default.GetBytes(paper.Message);

            return response;
        }
    }
}
