using Microsoft.EntityFrameworkCore;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;
using Core.Domain.Entities.Shop;

namespace Infra.Persistence.Context;

public partial class AppDbContext : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    #region CMS
    public virtual DbSet<Permission> Permissions { get; set; }
    #endregion

    #region Blog
    public virtual DbSet<Attachment> Attachments { get; set; }
    public virtual DbSet<AttachmentMeta> AttachmentMetas { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Comment> Comments { get; set; }
    public virtual DbSet<CommentMeta> CommentMetas { get; set; }
    public virtual DbSet<Page> Pages { get; set; }
    public virtual DbSet<PageMeta> PageMetas { get; set; }
    public virtual DbSet<Post> Posts { get; set; }
    public virtual DbSet<PostMeta> PostMetas { get; set; }
    public virtual DbSet<Option> Options { get; set; }
    #endregion

    #region Shop
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductMeta> ProductMetas { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<ReviewMeta> ReviewMetas { get; set; }
    public virtual DbSet<ProductCat> ProductCats { get; set; }

    //public virtual DbSet<Coupon> Coupons { get; set; }
    //public virtual DbSet<CouponMeta> CouponMetas { get; set; }
    //public virtual DbSet<Order> Orders { get; set; }
    //public virtual DbSet<Order> Orders { get; set; }
    //public virtual DbSet<OrderItem> OrderItems { get; set; }
    //public virtual DbSet<Transaction> Transactions { get; set; }
    //public virtual DbSet<Invoice> Invoices { get; set; }
    //public virtual DbSet<Tax> Taxes { get; set; }
    //public virtual DbSet<Shipping> Shippings { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region CMS Entities
        builder.Entity<Attachment>().HasIndex(c => c.Slug).IsUnique();
        builder.Entity<Post>().HasIndex(c => c.Slug).IsUnique();
        builder.Entity<Page>().HasIndex(c => c.Slug).IsUnique();
        builder.Entity<Category>().HasIndex(c => c.Slug).IsUnique();
        builder.Entity<Permission>().HasIndex(c => c.Name).IsUnique();
        #endregion

        #region Identity Entities
        builder.Entity<User>(b =>
        {
            b.ToTable("Users");

            // Each User can have many UserClaims
            b.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            b.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            b.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        });

        builder.Entity<Role>(b =>
        {
            b.ToTable("Roles");

            // Each Role can have many entries in the UserRole join table
            b.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each Role can have many associated RoleClaims
            b.HasMany(e => e.RoleClaims)
                .WithOne(e => e.Role)
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();
        });

        builder.Entity<UserClaim>(b =>
        {
            b.ToTable("UserClaims");
            b.HasKey(e => e.Id);
        });

        builder.Entity<UserLogin>(b =>
        {
            b.ToTable("UserLogins");
        });

        builder.Entity<UserToken>(b =>
        {
            b.ToTable("UserTokens");
        });

        builder.Entity<RoleClaim>(b =>
        {
            b.ToTable("RoleClaims");
        });

        builder.Entity<UserRole>(b =>
        {
            b.ToTable("UserRoles");
        });
        #endregion

        #region Shop Entities
        builder.Entity<Product>().HasIndex(c => c.Slug).IsUnique();
        #endregion
    }
}
