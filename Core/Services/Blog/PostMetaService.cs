using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Entities;
using Core.Domain.Entities.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class PostMetaService : BaseService<PostMeta>, IPostMetaService
{
    private readonly IPostMetaRepository _postMetaRepository;

    public PostMetaService(IPostMetaRepository postMetaRepository) : base(postMetaRepository)
    {
        _postMetaRepository = postMetaRepository;
    }

    public async Task<PostMeta> GetByIdAsync(int postId, string metaName)
    {
        return await _postMetaRepository.GetByIdAsync(postId, metaName);
    }

    public async Task<IEnumerable<PostMeta>> AddAsync(IEnumerable<PostMeta> metas)
    {
        try
        {
            return await _postMetaRepository.AddAsync(metas);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> AddTranslationMeta(Post currentPost)
    {
        try
        {
            var dictionary = new Dictionary<string, int>();
            var currentLanguageCode = currentPost.PostMetas.First(x => x.MetaName == "LanguageCode").MetaValue;
            dictionary.Add(currentLanguageCode, currentPost.Id);

            var newMeta = new PostMeta
            {
                PostId = currentPost.Id,
                MetaName = "Translation",
                MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary)
            };
            var result = await AddAsync(newMeta);
            if (result == null) return false;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateTranslationMeta(Post currentPost, Post parentPost)
    {
        try
        {
            var dictionary = new Dictionary<string, int>();
            //var parentPost = await _productService.GetByIdAsync(parentPostId);
            var meta = parentPost.PostMetas.FirstOrDefault(x => x.MetaName == "Translation");
            var parentLanguageCode = parentPost.PostMetas.First(x => x.MetaName == "LanguageCode").MetaValue;
            var currentLanguageCode = currentPost.PostMetas.First(x => x.MetaName == "LanguageCode").MetaValue;
            if (meta != null)
            {
                dictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, int>>(meta.MetaValue);
                dictionary.Add(currentLanguageCode, currentPost.Id);
            }
            else
            {
                dictionary.Add(parentLanguageCode, parentPost.Id);
                dictionary.Add(currentLanguageCode, currentPost.Id);
            }

            var ids = dictionary.Values.ToList();
            foreach (var id in ids)
            {
                var translationMeta = await GetByIdAsync(id, "Translation");
                if (translationMeta != null)
                {
                    translationMeta.MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary);
                    var result = await UpdateAsync(translationMeta);
                    if (result == null) return false;
                }
                else
                {
                    var newMeta = new PostMeta
                    {
                        PostId = id,
                        MetaName = "Translation",
                        MetaValue = Newtonsoft.Json.JsonConvert.SerializeObject(dictionary)
                    };
                    var result = await AddAsync(newMeta);
                    if (result == null) return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
