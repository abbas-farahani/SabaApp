using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Repositories;

public interface IOptionRepository : IBaseRepository<Option>
{
    Task<Option> GetByName(string name);
    Task<List<Option>> GetByNames(List<string> names);
    Task UpdateAsync(List<Option> options);

    Task<List<Option>> GetAllSliders();
    Task<Option> GetSlider(int sliderId);
}
