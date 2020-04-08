using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.DAL.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        public IRepository<T> Repository<T>() where T : class;
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
