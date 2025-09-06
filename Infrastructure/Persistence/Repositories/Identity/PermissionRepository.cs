using Core.Domain.Contracts.Repositories.Identity;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories.Identity;

public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
{
    private readonly AppDbContext _context;
    public PermissionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddAsync(List<Permission> permissions)
    {
        _context.Permissions.AddRange(permissions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Permission>> GetAllByName(string name)
    {
        return await _context.Permissions.Where(x=>x.Name.Contains(name)).ToListAsync();
    }
}
