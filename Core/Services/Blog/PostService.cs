using Core.Domain.Contracts.Repositories;
using Core.Domain.Contracts.Services;
using Core.Domain.Dtos.Blog;
using Core.Domain.Dtos.Shop;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services;

public class PostService : BaseService<Post>, IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IPostMetaService _postMetaService;
    private readonly ICategoryService _categoryService;
    public PostService(IPostRepository postRepository, IPostMetaService postMetaService, ICategoryService categoryService) : base(postRepository)
    {
        _postRepository = postRepository;
        _postMetaService = postMetaService;
        _categoryService = categoryService;
    }

    //public new async Task<PostDto> GetByIdAsync(int id)
    //{
    //    var post = await _postRepository.GetByIdAsync(id);
    //    return new PostDto
    //    {
    //        Id = post.Id,
    //        CreationTime = post.CreationTime,
    //        LastModified = post.LastModified,
    //        UserId = post.UserId,
    //        UserName = post.User.UserName,
    //        Title = post.Title,
    //        Content = post.Content,
    //        Slug = post.Slug,
    //        Comments = post.Comments,
    //        PostMetas = post.PostMetas,
    //        Categories = post.Categories,
    //        Status = (post.Status == 1) ? "published" : (post.Status == 3) ? "draft" : (post.Status == 4) ? "private" : (post.Status == 5) ? "scheduled" : "draft",
    //        CommentStatus = post.CommentStatus == 1 ? "open" : (post.CommentStatus == 2 ? "private" : "close"),
    //        ThumbPath = post.PostMetas.FirstOrDefault(x => x.MetaName == "ThumbPath")?.MetaValue,
    //        CategoriesId = post.Categories.Select(x => x.Id).ToList(),
    //    };
    //}

    public async Task<List<Post>> GetAllAsync(string lang)
    {
        return await _postRepository.GetAllAsync(lang);
    }

    public async Task<PostDto> GetFullItemByIdAsync(int id)
    {
        var post = await _postRepository.GetByIdAsync(id);

        return new PostDto
        {
            Id = post.Id,
            CreationTime = post.CreationTime,
            LastModified = post.LastModified,
            UserId = post.UserId,
            UserName = post.User.UserName,
            Title = post.Title,
            Content = post.Content,
            Slug = post.Slug,
            Comments = post.Comments,
            PostMetas = post.PostMetas,
            Categories = post.Categories,
            CommentStatus = post.CommentStatus == 1 ? "open" : (post.CommentStatus == 2 ? "private" : "close"),
            Status = (post.Status == 1) ? "published" : (post.Status == 3) ? "draft" : (post.Status == 4) ? "private" : (post.Status == 5) ? "scheduled" : "draft",
            ThumbPath = post.PostMetas.FirstOrDefault(x => x.MetaName == "ThumbPath")?.MetaValue,
            CategoriesId = post.Categories.Select(x => x.Id).ToList(),
        };
    }

    public override async Task<IEnumerable<Post>> GetAllAsync()
    {
        return await _postRepository.GetAllAsync();
    }

    public async Task<Post> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        return await _postRepository.GetBySlug(slug, cancellationToken);
    }

    public async Task<List<Post>> GetByUserId(string userId, CancellationToken cancellationToken)
    {
        return await _postRepository.GetByUserId(userId, cancellationToken);
    }

    public async Task<List<Post>> GetList(int count, string? culture)
    {
        return await _postRepository.GetList(count, culture);
    }

    public async Task<List<Post>> GetOriginalProducts(string defaultLanguage)
    {
        return await _postRepository.GetOriginalProducts(defaultLanguage);
    }

    public async Task<bool> IsExistBySlug(string slug)
    {
        return await _postRepository.IsExistBySlug(slug);
    }

    public async Task<Post> AddAsync(PostDto postDto)
    {
        try
        {
            #region Post
            Post post = new Post
            {
                CreationTime = DateTime.Now,
                UserId = postDto.UserId,
                Title = postDto.Title,
                Content = postDto.Content,
                Slug = postDto.Slug,
                CommentStatus = (byte)(postDto.CommentStatus == "open" ? 1 : (postDto.CommentStatus == "private" ? 2 : 0)),
                Status = (byte)(postDto.Status == "published" ? 1 : (postDto.Status == "edited" ? 2 : (postDto.Status == "private" ? 4 : (postDto.Status == "scheduled" ? 5 : 3)))),
                PostMetas = new List<PostMeta>(),
                Categories = new List<Category>(),
            };
            #endregion


            #region Post Metas
            if (!string.IsNullOrEmpty(postDto.ThumbPath))
                post.PostMetas.Add(new PostMeta { MetaName = "ThumbPath", MetaValue = postDto.ThumbPath, });
            if (!string.IsNullOrEmpty(postDto.LanguagesList))
                post.PostMetas.Add(new PostMeta { MetaName = "LanguageCode", MetaValue = postDto.LanguagesList, });
            #endregion


            #region Category
            if (postDto.CategoriesId != null)
            {
                var allCats = await _categoryService.GetAllAsync();
                var cats = allCats.Where(x => postDto.CategoriesId.Contains(x.Id)).ToList();
                foreach (var cat in cats)
                {
                    post.Categories.Add(cat);
                }
            }
            #endregion

            return await this.AddAsync(post);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<Post> UpdateAsync(PostDto postDto)
    {
        try
        {
            var post = await this.GetByIdAsync(postDto.Id.Value);
            if (post == null) return null;

            post.LastModified = DateTime.Now;
            post.Title = postDto.Title;
            post.Content = postDto.Content;
            post.Slug = postDto.Slug;
            post.CommentStatus = (byte)(postDto.CommentStatus == "open" ? 1 : (postDto.CommentStatus == "private" ? 2 : 0));
            post.Status = (byte)(postDto.Status == "published" ? 1 : (postDto.Status == "edited" ? 2 : (postDto.Status == "private" ? 4 : (postDto.Status == "scheduled" ? 5 : 3))));

            var metas = post.PostMetas;
            post.PostMetas.Clear();
            if (postDto.ThumbPath != null)
            {
                if (metas.Any(x => x.MetaName == "ThumbPath"))
                    metas.Remove(metas.First(x => x.MetaName == "ThumbPath")); // Remove oldItem
                post.PostMetas.Add(new PostMeta
                {
                    MetaName = "ThumbPath",
                    MetaValue = postDto.ThumbPath,
                });
            }

            post.Categories.Clear();
            if (postDto.CategoriesId != null)
            {
                var allCats = await _categoryService.GetAllAsync();
                var cats = allCats.Where(x => postDto.CategoriesId.Contains(x.Id)).ToList();
                foreach (var cat in cats)
                {
                    post.Categories.Add(cat);
                }
            }
            return await this.UpdateAsync(post);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}
