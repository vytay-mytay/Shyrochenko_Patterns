using ShyrochenkoPatterns.Models.Facade;
using ShyrochenkoPatterns.Services.Interfaces.Facade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Facade
{
    public class Copier : ICopier
    {
        private IPrinter _printer;
        private IScaner _scaner;

        public Copier(IPrinter printer, IScaner scaner)
        {
            _printer = printer;
            _scaner = scaner;
        }

        public async Task<List<Paper>> Copy(Paper paper, int count)
        {
            var array = await _scaner.Scan(paper);

            var response = new List<Paper>();

            for (int i = 0; i < count; i++)
                response.Add(await _printer.Print(array));

            return response;
        }
    }
}
