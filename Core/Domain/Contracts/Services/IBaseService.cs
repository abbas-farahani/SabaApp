using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface IBaseService<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<List<T>> AddAsync(List<T> entities);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}