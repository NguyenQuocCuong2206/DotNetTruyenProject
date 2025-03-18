using DotNetTruyen.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace DotNetTruyen.Data
{
        public class DotNetTruyenDbContext : IdentityDbContext<

        User,
        IdentityRole<Guid>,
        Guid,
        IdentityUserClaim<Guid>,
        IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >
        {
            public DbSet<Advertisement> Advertisements { get; set; }
            public DbSet<Genre> Genres { get; set; }
            public DbSet<Comment> Comments { get; set; }
            public DbSet<Comic> Comics { get; set; }
            public DbSet<ComicGenre> ComicGenres { get; set; }
            public DbSet<Chapter> Chapters { get; set; }
            public DbSet<ChapterImage> ChapterImages { get; set; }
            public DbSet<Follow> Follows { get; set; }
            public DbSet<Like> Likes { get; set; }
            public DbSet<Notify> Notifies { get; set; }
            public DbSet<Rank> Ranks { get; set; }
            public DbSet<RankType> RankTypes { get; set; }
            public DbSet<ReadHistory> ReadHistories { get; set; }
            public DbSet<UserRank> UserRanks { get; set; }

            public DotNetTruyenDbContext(DbContextOptions<DotNetTruyenDbContext> options) : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Configure composite keys first
                modelBuilder.Entity<IdentityUserLogin<Guid>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
                modelBuilder.Entity<IdentityUserRole<Guid>>().HasKey(r => new { r.UserId, r.RoleId });
                modelBuilder.Entity<IdentityUserToken<Guid>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                // Then rename the tables
                modelBuilder.Entity<User>().ToTable("Users");
                modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
                modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
                modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
                modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
                modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
                modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

                

                // Your other entity configurations
                modelBuilder.Entity<ComicGenre>(entity =>
                {
                    entity.HasKey(x => new { x.ComicId, x.GenreId });
                });

                modelBuilder.Entity<Follow>(entity =>
                {
                    entity.HasKey(x => new { x.ComicId, x.UserId });
                });

                modelBuilder.Entity<UserRank>(entity =>
                {
                    entity.HasKey(x => new { x.UserId, x.RankId });
                });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(x => new { x.UserIpHash, x.ComicId });
            });

            // Tạo Role mặc định
            var adminRoleId = Guid.NewGuid();
            var userRoleId = Guid.NewGuid();

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid> { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole<Guid> { Id = userRoleId, Name = "Reader", NormalizedName = "READER" }
            );

            // Thêm tài khoản Admin
            var adminId = Guid.NewGuid();
            var adminEmail = "admin@example.com";
            var adminPassword = "Admin@123"; // Đặt mật khẩu mạnh

            var adminUser = new User
            {
                Id = adminId,
                UserName = "admin",
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            // Mã hóa mật khẩu (dùng PasswordHasher)
            var passwordHasher = new PasswordHasher<User>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, adminPassword);

            modelBuilder.Entity<User>().HasData(adminUser);

            // Gán tài khoản Admin vào Role "Admin"
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    UserId = adminId,
                    RoleId = adminRoleId
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEnity<Guid>>())
            {
                
                    switch (entry.State)
                    {
                        case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow; 
                            break;

                        case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                            break;

                        case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.DeletedAt = DateTime.UtcNow; 
                            break;
                    }
               
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}

