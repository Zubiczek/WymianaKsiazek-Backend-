using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WymianaKsiazek.Api.Database.Entities;
using WymianaKsiazek.Api.Database.Extensions;

namespace WymianaKsiazek.Api.Database
{
    public class Context : IdentityDbContext<UserEntity>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        // kolejne tabele w bazie
        public DbSet<BookEntity> Books { get; set; }

        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyAllConfigurations();
        }
    }
}