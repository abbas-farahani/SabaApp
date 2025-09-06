using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class BaseService<T> : IBaseService<T> where T : class
{
    private readonly IBaseRepository<T> _repository;

    public BaseService(IBaseRepository<T> repository)
    {
        _repository = repository;
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        try
        {
            return await _repository.CreateAsync(entity);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public virtual async Task<List<T>> AddAsync(List<T> entities)
    {
        try
        {
            return await _repository.CreateAsync(entities);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        try
        {
            return await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }
}
