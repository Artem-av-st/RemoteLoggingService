﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RemoteLoggingService.Models;

namespace RemoteLoggingService.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180906083307_migration")]
    partial class migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.2-rtm-30932")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RemoteLoggingService.Models.Client", b =>
                {
                    b.Property<Guid?>("ClientId");

                    b.Property<Guid?>("DeveloperUserId");

                    b.HasKey("ClientId");

                    b.HasIndex("DeveloperUserId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("RemoteLoggingService.Models.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("ClientGuid");

                    b.Property<string>("Message")
                        .IsRequired();

                    b.Property<string>("Metadata");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("ClientGuid");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("RemoteLoggingService.Models.User", b =>
                {
                    b.Property<Guid?>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email");

                    b.Property<bool>("IsApproved");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<int?>("UserRoleId");

                    b.HasKey("UserId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RemoteLoggingService.Models.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("RemoteLoggingService.Models.Client", b =>
                {
                    b.HasOne("RemoteLoggingService.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("RemoteLoggingService.Models.User", "Developer")
                        .WithMany()
                        .HasForeignKey("DeveloperUserId");
                });

            modelBuilder.Entity("RemoteLoggingService.Models.Log", b =>
                {
                    b.HasOne("RemoteLoggingService.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("ClientGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("RemoteLoggingService.Models.User", b =>
                {
                    b.HasOne("RemoteLoggingService.Models.UserRole", "UserRole")
                        .WithMany("Users")
                        .HasForeignKey("UserRoleId");
                });
#pragma warning restore 612, 618
        }
    }
}
