using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Marketing.Api.Services
{
    public interface IDynamoDbContext<T> : IDisposable where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task SaveAsync(T item);
        Task DeleteByIdAsync(T item);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetByIdAsync(string id, CancellationToken cancellationToken);

    }
}