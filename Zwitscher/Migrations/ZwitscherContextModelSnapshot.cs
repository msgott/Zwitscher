﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zwitscher.Data;

#nullable disable

namespace Zwitscher.Migrations
{
    [DbContext(typeof(ZwitscherContext))]
    partial class ZwitscherContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UserUser", b =>
                {
                    b.Property<Guid>("FollowedById")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FollowingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FollowedById", "FollowingId");

                    b.HasIndex("FollowingId");

                    b.ToTable("UserFollowers", (string)null);
                });

            modelBuilder.Entity("UserUser1", b =>
                {
                    b.Property<Guid>("BlockedById")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("BlockingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("BlockedById", "BlockingId");

                    b.HasIndex("BlockingId");

                    b.ToTable("UserBlockers", (string)null);
                });

            modelBuilder.Entity("Zwitscher.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CommentText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("commentsCommentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.HasIndex("commentsCommentId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("Zwitscher.Models.Media", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Media");
                });

            modelBuilder.Entity("Zwitscher.Models.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("TextContent")
                        .IsRequired()
                        .HasMaxLength(281)
                        .HasColumnType("nvarchar(281)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("retweetsID")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("retweetsID");

                    b.ToTable("Post");
                });

            modelBuilder.Entity("Zwitscher.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Zwitscher.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Biography")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Birthday")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid?>("MediaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RoleID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("isLocked")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("MediaId")
                        .IsUnique()
                        .HasFilter("[MediaId] IS NOT NULL");

                    b.HasIndex("RoleID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Zwitscher.Models.Vote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("isUpVote")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId", "PostId")
                        .IsUnique();

                    b.ToTable("Vote");
                });

            modelBuilder.Entity("UserUser", b =>
                {
                    b.HasOne("Zwitscher.Models.User", null)
                        .WithMany()
                        .HasForeignKey("FollowedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zwitscher.Models.User", null)
                        .WithMany()
                        .HasForeignKey("FollowingId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserUser1", b =>
                {
                    b.HasOne("Zwitscher.Models.User", null)
                        .WithMany()
                        .HasForeignKey("BlockedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zwitscher.Models.User", null)
                        .WithMany()
                        .HasForeignKey("BlockingId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Zwitscher.Models.Comment", b =>
                {
                    b.HasOne("Zwitscher.Models.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId");

                    b.HasOne("Zwitscher.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zwitscher.Models.Comment", "commentsComment")
                        .WithMany("commentedBy")
                        .HasForeignKey("commentsCommentId");

                    b.Navigation("Post");

                    b.Navigation("User");

                    b.Navigation("commentsComment");
                });

            modelBuilder.Entity("Zwitscher.Models.Media", b =>
                {
                    b.HasOne("Zwitscher.Models.Post", "Post")
                        .WithMany("Media")
                        .HasForeignKey("PostId");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("Zwitscher.Models.Post", b =>
                {
                    b.HasOne("Zwitscher.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zwitscher.Models.Post", "retweets")
                        .WithMany()
                        .HasForeignKey("retweetsID");

                    b.Navigation("User");

                    b.Navigation("retweets");
                });

            modelBuilder.Entity("Zwitscher.Models.User", b =>
                {
                    b.HasOne("Zwitscher.Models.Media", "ProfilePicture")
                        .WithOne("User")
                        .HasForeignKey("Zwitscher.Models.User", "MediaId");

                    b.HasOne("Zwitscher.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleID");

                    b.Navigation("ProfilePicture");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Zwitscher.Models.Vote", b =>
                {
                    b.HasOne("Zwitscher.Models.Post", "Post")
                        .WithMany("Votes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zwitscher.Models.User", "User")
                        .WithMany("Votes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Zwitscher.Models.Comment", b =>
                {
                    b.Navigation("commentedBy");
                });

            modelBuilder.Entity("Zwitscher.Models.Media", b =>
                {
                    b.Navigation("User");
                });

            modelBuilder.Entity("Zwitscher.Models.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Media");

                    b.Navigation("Votes");
                });

            modelBuilder.Entity("Zwitscher.Models.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Zwitscher.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Posts");

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
