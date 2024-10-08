﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SpraywallAppWeb.Data;

#nullable disable

namespace SpraywallAppWeb.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20240831072502_WallRelations2SavingBanning")]
    partial class WallRelations2SavingBanning
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.8");

            modelBuilder.Entity("SpraywallAppWeb.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("SpraywallAppWeb.Models.Wall", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AccessibleViaSearch")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IdentifiedHoldsJsonPath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ManagerID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ManagerID");

                    b.ToTable("Walls");
                });

            modelBuilder.Entity("UserWall", b =>
                {
                    b.Property<int>("BannedUsersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("BannedWallsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("BannedUsersId", "BannedWallsId");

                    b.HasIndex("BannedWallsId");

                    b.ToTable("UserWall");
                });

            modelBuilder.Entity("UserWall1", b =>
                {
                    b.Property<int>("SavedUsersId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SavedWallsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("SavedUsersId", "SavedWallsId");

                    b.HasIndex("SavedWallsId");

                    b.ToTable("UserWall1");
                });

            modelBuilder.Entity("SpraywallAppWeb.Models.Wall", b =>
                {
                    b.HasOne("SpraywallAppWeb.Models.User", "Manager")
                        .WithMany("ManagedWalls")
                        .HasForeignKey("ManagerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("UserWall", b =>
                {
                    b.HasOne("SpraywallAppWeb.Models.User", null)
                        .WithMany()
                        .HasForeignKey("BannedUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SpraywallAppWeb.Models.Wall", null)
                        .WithMany()
                        .HasForeignKey("BannedWallsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("UserWall1", b =>
                {
                    b.HasOne("SpraywallAppWeb.Models.User", null)
                        .WithMany()
                        .HasForeignKey("SavedUsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SpraywallAppWeb.Models.Wall", null)
                        .WithMany()
                        .HasForeignKey("SavedWallsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SpraywallAppWeb.Models.User", b =>
                {
                    b.Navigation("ManagedWalls");
                });
#pragma warning restore 612, 618
        }
    }
}
