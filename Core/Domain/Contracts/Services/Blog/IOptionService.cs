using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Contracts.Services;

public interface IOptionService : IBaseService<Option>
{
    Task<string> GetByName(string name);
    Task<List<Option>> GetByNames(List<string> names);
    Task<List<Option>> prepareByNames(List<string> names);
    Task UpdateAsync(List<Option> options);

    Task<List<SliderDto>> GetAllSliders();
    Task<SliderDto> GetSlider(int sliderId);
    Task<SliderDto> GetSliderByName(string sliderName);
    Task<Option> AddSlider(SliderDto slider);
    Task<Option> UpdateSlider(SliderDto slider);
    Task DeleteSlider(int sliderId);
    Task DeleteSlide(int sliderId, int slideId);
}
