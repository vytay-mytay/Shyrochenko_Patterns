using ShyrochenkoPatterns.Models.Facade;
using ShyrochenkoPatterns.Services.Interfaces.Facade;
using System.Text;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Facade
{
    public class Printer : IPrinter
    {
        public async Task<Paper> Print(byte[] array)
        {
            var response = new Paper(Encoding.Default.GetString(array));

            return response;
        }
    }
}
