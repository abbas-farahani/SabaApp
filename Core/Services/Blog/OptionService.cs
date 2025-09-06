using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Dtos.Blog;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class OptionService : BaseService<Option>, IOptionService
{
    private readonly IOptionRepository _optionRepository;

    public OptionService(IOptionRepository optionRepository) : base(optionRepository)
    {
        _optionRepository = optionRepository;
    }

    public async Task<string> GetByName(string name)
    {
        var option = await _optionRepository.GetByName(name);
        if (option == null) return "";
        return option.OptionValue;
    }

    public async Task<List<Option>> GetByNames(List<string> names)
    {
        return await _optionRepository.GetByNames(names);
    }

    public async Task<List<Option>> prepareByNames(List<string> names)
    {
        var options = await _optionRepository.GetByNames(names);
        var ex = names.Where(item => !(options.Select(x => x.OptionName).Contains(item))).ToList();
        var newItems = new List<Option>();
        foreach (var item in ex)
        {
            newItems.Add(new Option
            {
                OptionName = item,
                OptionValue = ""
            });
        }
        await _optionRepository.CreateAsync(newItems);
        return await _optionRepository.GetByNames(names);
    }


    public override async Task<Option> UpdateAsync(Option option)
    {
        var item = await _optionRepository.GetByName(option.OptionName);

        if (item == null || item.OptionName != option.OptionName)
        {
            await _optionRepository.CreateAsync(option);
        }
        else
        {
            item.OptionValue = option.OptionValue;
            await _optionRepository.UpdateAsync(item);
        }
        return option;
    }

    public async Task UpdateAsync(List<Option> options)
    {
        var names = options.Select(x => x.OptionName).ToList();
        var items = await _optionRepository.GetByNames(names);

        foreach (var option in options)
        {
            if (!(items.Any(x => x.OptionName == option.OptionName)))
            {
                await _optionRepository.CreateAsync(option);
                options.Remove(option);
            }
            else
            {
                items.First(x => x.OptionName == option.OptionName).OptionValue = option.OptionValue;
            }
        }
        await _optionRepository.UpdateAsync(items);
    }


    public async Task<List<SliderDto>> GetAllSliders()
    {
        var sliders = new List<SliderDto>();
        var options = await _optionRepository.GetAllSliders();
        foreach (var option in options)
        {
            try
            {
                sliders.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<SliderDto>(option.OptionValue));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        return sliders;
    }

    public async Task<SliderDto> GetSlider(int sliderId)
    {
        var option = await _optionRepository.GetSlider(sliderId);
        return Newtonsoft.Json.JsonConvert.DeserializeObject<SliderDto>(option.OptionValue);
    }

    public async Task<SliderDto> GetSliderByName(string sliderName)
    {
        var option = await _optionRepository.GetAllSliders();
        var slide = option.Select(x =>
        {
            var value = Newtonsoft.Json.JsonConvert.DeserializeObject<SliderDto>(x.OptionValue);
            if (value.Name == sliderName)
                return value;
            return null;
        }).FirstOrDefault();
        return slide;
    }


    public Task<Option> AddSlider(SliderDto slider)
    {
        var option = new Option
        {
            OptionName = $"Slider_{slider.Id}",
            OptionValue = Newtonsoft.Json.JsonConvert.SerializeObject(slider),
        };
        return this.UpdateAsync(option);
    }

    public async Task<Option> UpdateSlider(SliderDto slider)
    {
        if (slider.Id == 0 || slider.Id == null)
        {
            slider.Id = 1;
            var all = await GetAllSliders();
            if (all.Count() > 0)
            {
                all.Select(x =>
                {
                    if (x.Id >= slider.Id) slider.Id = x.Id + 1;
                    return x;
                }).ToList();
            }
        }
        var option = new Option
        {
            OptionName = $"Slider_{slider.Id}",
            OptionValue = Newtonsoft.Json.JsonConvert.SerializeObject(slider),
        };
        return await this.UpdateAsync(option);
    }

    public async Task DeleteSlider(int sliderId)
    {
        var slider = await _optionRepository.GetSlider(sliderId);
        var result = await this.DeleteAsync(slider.Id);
    }

    public async Task DeleteSlide(int sliderId, int slideId)
    {
        var option = await _optionRepository.GetSlider(sliderId);
        var slider = Newtonsoft.Json.JsonConvert.DeserializeObject<SliderDto>(option.OptionValue);
        var slide = slider.Slides.Find(x => x.Id == slideId);
        if (slider != null && slide != null) slider.Slides.Remove(slide);
        var update = await this.UpdateSlider(slider);
    }
}

