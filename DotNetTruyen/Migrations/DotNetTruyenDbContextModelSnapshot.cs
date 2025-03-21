﻿// <auto-generated />
using System;
using DotNetTruyen.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DotNetTruyen.Migrations
{
    [DbContext(typeof(DotNetTruyenDbContext))]
    partial class DotNetTruyenDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DotNetTruyen.Models.Advertisement", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("ImageUrl")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("LinkTo")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Advertisements");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Chapter", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("ChapterNumber")
                    .HasColumnType("int");

                b.Property<string>("ChapterTitle")
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("ComicId")
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<bool>("IsPublished")
                    .HasColumnType("bit");

                b.Property<DateTime?>("PublishedDate")
                    .HasColumnType("datetime2");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("Views")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("ComicId");

                b.ToTable("Chapters");
            });

            modelBuilder.Entity("DotNetTruyen.Models.ChapterImage", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("ChapterId")
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<int>("ImageNumber")
                    .HasColumnType("int");

                b.Property<string>("ImageUrl")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("ChapterId");

                b.ToTable("ChapterImages");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Comic", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Author")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("CoverImage")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("Likes")
                    .HasColumnType("int");

                b.Property<bool>("Status")
                    .HasColumnType("bit");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("View")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.ToTable("Comics");
            });

            modelBuilder.Entity("DotNetTruyen.Models.ComicGenre", b =>
            {
                b.Property<Guid>("ComicId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("GenreId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("ComicId", "GenreId");

                b.HasIndex("GenreId");

                b.ToTable("ComicGenres");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Comment", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("ComicId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid?>("CommentId")
                    .HasColumnType("uniqueidentifier")
                    .HasColumnName("ReplyId");

                b.Property<string>("Content")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime>("Date")
                    .HasColumnType("datetime2");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("ComicId");

                b.HasIndex("CommentId");

                b.HasIndex("UserId");

                b.ToTable("Comments");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Follow", b =>
            {
                b.Property<Guid>("ComicId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("ComicId", "UserId");

                b.HasIndex("UserId");

                b.ToTable("Follows");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Genre", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("GenreName")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Genres");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Notification", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Icon")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("IsRead")
                    .HasColumnType("bit");

                b.Property<string>("Link")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Message")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Type")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("Notification");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Rank", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<int>("ExpRequired")
                    .HasColumnType("int");

                b.Property<int>("Level")
                    .HasColumnType("int");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("RankTypeId")
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.HasIndex("RankTypeId");

                b.ToTable("Ranks");
            });

            modelBuilder.Entity("DotNetTruyen.Models.RankType", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime?>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("CreatedBy")
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("DeletedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<DateTime?>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("UpdatedBy")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("Id");

                b.ToTable("RankTypes");
            });

            modelBuilder.Entity("DotNetTruyen.Models.ReadHistory", b =>
            {
                b.Property<Guid>("ReadHistoryId")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("ChapterId")
                    .HasColumnType("uniqueidentifier");

                b.Property<bool>("IsRead")
                    .HasColumnType("bit");

                b.Property<DateTime>("ReadDate")
                    .HasColumnType("datetime2");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("ReadHistoryId");

                b.HasIndex("ChapterId");

                b.HasIndex("UserId");

                b.ToTable("ReadHistories");
            });

            modelBuilder.Entity("DotNetTruyen.Models.User", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("AccessFailedCount")
                    .HasColumnType("int");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Email")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<bool>("EmailConfirmed")
                    .HasColumnType("bit");

                b.Property<string>("ImageUrl")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("LockoutEnabled")
                    .HasColumnType("bit");

                b.Property<DateTimeOffset?>("LockoutEnd")
                    .HasColumnType("datetimeoffset");

                b.Property<string>("NameToDisplay")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("NormalizedEmail")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<string>("NormalizedUserName")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<string>("PasswordHash")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("PhoneNumberConfirmed")
                    .HasColumnType("bit");

                b.Property<string>("SecurityStamp")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("TwoFactorEnabled")
                    .HasColumnType("bit");

                b.Property<string>("UserName")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.HasKey("Id");

                b.HasIndex("NormalizedEmail")
                    .HasDatabaseName("EmailIndex");

                b.HasIndex("NormalizedUserName")
                    .IsUnique()
                    .HasDatabaseName("UserNameIndex")
                    .HasFilter("[NormalizedUserName] IS NOT NULL");

                b.ToTable("Users", (string)null);

                b.HasData(
                    new
                    {
                        Id = new Guid("b6b72342-5016-4c6a-829c-b3b4f8c317ab"),
                        AccessFailedCount = 0,
                        ConcurrencyStamp = "220bb9d3-653a-4777-a837-ca62871deda7",
                        Email = "admin@example.com",
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        NormalizedEmail = "ADMIN@EXAMPLE.COM",
                        NormalizedUserName = "ADMIN",
                        PasswordHash = "AQAAAAIAAYagAAAAEBWRf2LM/CyAUIURYdJRRaRl9jGNVfiQ+IX3gVvbEY0SUwO3/Cju5Gi8Y5pnpGNApw==",
                        PhoneNumberConfirmed = false,
                        SecurityStamp = "2ae36440-2771-4c20-ac5a-0b98040a937b",
                        TwoFactorEnabled = false,
                        UserName = "admin"
                    });
            });

            modelBuilder.Entity("DotNetTruyen.Models.UserRank", b =>
            {
                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("RankId")
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("Exp")
                    .HasColumnType("int");

                b.HasKey("UserId", "RankId");

                b.HasIndex("RankId");

                b.ToTable("UserRanks");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Name")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.Property<string>("NormalizedName")
                    .HasMaxLength(256)
                    .HasColumnType("nvarchar(256)");

                b.HasKey("Id");

                b.HasIndex("NormalizedName")
                    .IsUnique()
                    .HasDatabaseName("RoleNameIndex")
                    .HasFilter("[NormalizedName] IS NOT NULL");

                b.ToTable("Roles", (string)null);

                b.HasData(
                    new
                    {
                        Id = new Guid("0d193d65-6228-4327-b269-b6d94c8f92e1"),
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    },
                    new
                    {
                        Id = new Guid("eaf063f9-3500-4b3a-a533-a54f6baa3f94"),
                        Name = "Reader",
                        NormalizedName = "READER"
                    });
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("ClaimType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("RoleId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("RoleClaims", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                b.Property<string>("ClaimType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("UserClaims", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
            {
                b.Property<string>("LoginProvider")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("ProviderKey")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("ProviderDisplayName")
                    .HasColumnType("nvarchar(max)");

                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("LoginProvider", "ProviderKey");

                b.HasIndex("UserId");

                b.ToTable("UserLogins", (string)null);
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
            {
                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("RoleId")
                    .HasColumnType("uniqueidentifier");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.ToTable("UserRoles", (string)null);

                b.HasData(
                    new
                    {
                        UserId = new Guid("b6b72342-5016-4c6a-829c-b3b4f8c317ab"),
                        RoleId = new Guid("0d193d65-6228-4327-b269-b6d94c8f92e1")
                    });
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
            {
                b.Property<Guid>("UserId")
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("LoginProvider")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("Value")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("UserId", "LoginProvider", "Name");

                b.ToTable("UserTokens", (string)null);
            });

            modelBuilder.Entity("DotNetTruyen.Models.Chapter", b =>
            {
                b.HasOne("DotNetTruyen.Models.Comic", "Comic")
                    .WithMany("Chapters")
                    .HasForeignKey("ComicId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Comic");
            });

            modelBuilder.Entity("DotNetTruyen.Models.ChapterImage", b =>
            {
                b.HasOne("DotNetTruyen.Models.Chapter", "Chapter")
                    .WithMany("Images")
                    .HasForeignKey("ChapterId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Chapter");
            });

            modelBuilder.Entity("DotNetTruyen.Models.ComicGenre", b =>
            {
                b.HasOne("DotNetTruyen.Models.Comic", "Comic")
                    .WithMany("ComicGenres")
                    .HasForeignKey("ComicId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("DotNetTruyen.Models.Genre", "Genre")
                    .WithMany("ComicGenres")
                    .HasForeignKey("GenreId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Comic");

                b.Navigation("Genre");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Comment", b =>
            {
                b.HasOne("DotNetTruyen.Models.Comic", "Comic")
                    .WithMany()
                    .HasForeignKey("ComicId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("DotNetTruyen.Models.Comment", null)
                    .WithMany("Comments")
                    .HasForeignKey("CommentId");

                b.HasOne("DotNetTruyen.Models.User", "User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Comic");

                b.Navigation("User");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Follow", b =>
            {
                b.HasOne("DotNetTruyen.Models.Comic", "Comic")
                    .WithMany("Follows")
                    .HasForeignKey("ComicId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("DotNetTruyen.Models.User", "User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Comic");

                b.Navigation("User");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Rank", b =>
            {
                b.HasOne("DotNetTruyen.Models.RankType", "RankType")
                    .WithMany()
                    .HasForeignKey("RankTypeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("RankType");
            });

            modelBuilder.Entity("DotNetTruyen.Models.ReadHistory", b =>
            {
                b.HasOne("DotNetTruyen.Models.Chapter", "Chapter")
                    .WithMany()
                    .HasForeignKey("ChapterId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("DotNetTruyen.Models.User", "User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Chapter");

                b.Navigation("User");
            });

            modelBuilder.Entity("DotNetTruyen.Models.UserRank", b =>
            {
                b.HasOne("DotNetTruyen.Models.Rank", "Rank")
                    .WithMany("UserRanks")
                    .HasForeignKey("RankId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("DotNetTruyen.Models.User", "User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.Navigation("Rank");

                b.Navigation("User");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
            {
                b.HasOne("DotNetTruyen.Models.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
            {
                b.HasOne("DotNetTruyen.Models.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<System.Guid>", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("DotNetTruyen.Models.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
            {
                b.HasOne("DotNetTruyen.Models.User", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("DotNetTruyen.Models.Chapter", b =>
            {
                b.Navigation("Images");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Comic", b =>
            {
                b.Navigation("Chapters");

                b.Navigation("ComicGenres");

                b.Navigation("Follows");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Comment", b =>
            {
                b.Navigation("Comments");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Genre", b =>
            {
                b.Navigation("ComicGenres");
            });

            modelBuilder.Entity("DotNetTruyen.Models.Rank", b =>
            {
                b.Navigation("UserRanks");
            });
#pragma warning restore 612, 618
        }
    }
}


