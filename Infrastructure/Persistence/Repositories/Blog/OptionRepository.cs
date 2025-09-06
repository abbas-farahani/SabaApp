using Core.Domain.Contracts.Repositories;
using Core.Domain.Entities;
using Infra.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Persistence.Repositories;

public class OptionRepository : BaseRepository<Option>, IOptionRepository
{
    private readonly AppDbContext _context;
    public OptionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Option> GetByName(string name)
    {
        return await _context.Options.Where(x => x.OptionName == name).FirstOrDefaultAsync();
    }

    public async Task<List<Option>> GetByNames(List<string> names)
    {
        return await _context.Options.Where(x => names.Contains(x.OptionName)).ToListAsync();
    }

    public async Task UpdateAsync(List<Option> options)
    {
        _context.Options.UpdateRange(options);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Option>> GetAllSliders()
    {
        return await _context.Options.Where(x => x.OptionName.StartsWith("Slider_")).ToListAsync();
    }

    public async Task<Option> GetSlider(int sliderId)
    {
        return await _context.Options.Where(x => x.OptionName == $"Slider_{sliderId}").FirstOrDefaultAsync();
    }
}