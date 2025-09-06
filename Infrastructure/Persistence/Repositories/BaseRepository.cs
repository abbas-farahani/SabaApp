using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
	private readonly ConcurrentDictionary<string, object> _entitySets = new ConcurrentDictionary<string, object>();
    
	protected AppDbContext _context { get; private set; }

	protected BaseRepository(AppDbContext context)
	{
		_context = context;
	}

    #region Methods (Model I)
    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<List<T>> CreateAsync(List<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();
        return entities;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Methods (Model II)
    //public virtual void Insert<T>(T entity) where T : BaseEntity
    //{
    //	if (entity == null)
    //	{
    //		throw new ArgumentNullException(nameof(entity));
    //	}

    //	Entities<T>().Add(entity);
    //}

    //protected virtual void Insert<T>(IEnumerable<T> entities) where T : BaseEntity
    //{
    //	if (entities?.Any() != true)
    //	{
    //		throw new ArgumentException(nameof(entities));
    //	}

    //	Entities<T>().AddRange(entities);
    //}

    //protected virtual void Update<T>(T entity)
    //	where T : BaseEntity
    //{
    //	if (entity == null)
    //	{
    //		throw new ArgumentNullException(nameof(entity));
    //	}

    //	var entry = _context.Entry(entity);
    //	if (entry == null)
    //	{
    //		var cachedEntry = _context.ChangeTracker.Entries<T>().FirstOrDefault(x => x.Entity.Id == entity.Id);
    //		if (cachedEntry != null)
    //		{
    //			cachedEntry.State = EntityState.Detached;
    //		}

    //		Entities<T>().Attach(entity);
    //		entry = _context.Entry(entity);
    //	}

    //	entry.State = EntityState.Modified;
    //}

    //protected virtual void Update<T>(IEnumerable<T> entities)
    //	where T : BaseEntity
    //{
    //	if (entities?.Any() != true)
    //	{
    //		throw new ArgumentException(nameof(entities));
    //	}

    //	Entities<T>().AttachRange(entities);
    //	foreach (var entity in entities)
    //	{
    //		_context.Entry(entity).State = EntityState.Modified;
    //	}
    //}

    //protected virtual void Delete<T>(T entity)
    //	where T : BaseEntity
    //{
    //	if (entity == null)
    //	{
    //		throw new ArgumentNullException(nameof(entity));
    //	}

    //	Entities<T>().Remove(entity);
    //}

    //protected List<T> RawSqlQuery<T>(string query, Func<DbDataReader, T> map)
    //{
    //	using var command = _context.Database.GetDbConnection().CreateCommand();
    //	command.CommandText = query;
    //	command.CommandType = CommandType.Text;

    //	_context.Database.OpenConnection();

    //	using var result = command.ExecuteReader();
    //	var entities = new List<T>();

    //	while (result.Read())
    //	{
    //		entities.Add(map(result));
    //	}

    //	return entities;
    //}

    //protected virtual void Delete<T>(IEnumerable<T> entities)
    //	where T : BaseEntity
    //{
    //	if (entities?.Any() != true)
    //	{
    //		throw new ArgumentException(nameof(entities));
    //	}

    //	Entities<T>().RemoveRange(entities);
    //}

    //protected Task<int> Save()
    //{
    //	var result = _context.SaveChangesAsync();

    //	return result;
    //}

    //protected virtual IQueryable<T> Table<T>() where T : BaseEntity
    //{
    //	return Entities<T>().AsNoTracking();
    //}

    //protected virtual IQueryable<T> TableTracking<T>()
    //	where T : BaseEntity
    //{
    //	return Entities<T>();
    //}

    //protected virtual DbSet<T> Entities<T>()
    //	where T : BaseEntity
    //{
    //	return _entitySets.GetOrAdd(typeof(T).FullName, (key) => { return _context.Set<T>(); }) as DbSet<T>;
    //}

    //protected virtual Task<T> GetById<T>(long entityId) where T : BaseEntity
    //{
    //	return Table<T>().SingleOrDefaultAsync(x => x.Id == entityId);
    //}

    //protected virtual Task<T> GetById<T>(long entityId, params string[] includs) where T : BaseEntity
    //{
    //	IQueryable<T> table = _context.Set<T>();
    //	foreach (var inc in includs)
    //	{
    //		table = table.Include(inc);
    //	}

    //	return table.SingleOrDefaultAsync(x => x.Id == entityId);
    //}

    #endregion
}
