using ShyrochenkoPatterns.Models.Facade;
using ShyrochenkoPatterns.Services.Interfaces.Facade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Facade
{
    public class MFU : IMFU
    {
        private ICopier _copier;
        private IScaner _scaner;
        private IPrinter _printer;

        public MFU(ICopier copier, IScaner scaner, IPrinter printer)
        {
            _copier = copier;
            _scaner = scaner;
            _printer = printer;
        }

        public async Task<List<Paper>> Copy(Paper paper, int count)
        {
            return await _copier.Copy(paper, count);
        }

        public async Task<Paper> Print(byte[] array)
        {
            return await _printer.Print(array);
        }

        public async Task<byte[]> Scan(Paper paper)
        {
            return await _scaner.Scan(paper);
        }
    }
}
