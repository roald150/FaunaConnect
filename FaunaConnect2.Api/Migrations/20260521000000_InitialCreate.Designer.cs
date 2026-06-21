using System;
using FaunaConnect2.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FaunaConnect2.Api.Migrations
{
    [DbContext(typeof(FaunaDbContext))]
    [Migration("20260521000000_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "10.0.8");

            modelBuilder.Entity("FaunaConnect2.Api.Models.AnimalSpecies", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IconUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AnimalSpecies");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsGroupChat")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ReceiverId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SenderId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ReceiverId");

                    b.HasIndex("SenderId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.DamageReport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("HuntingGroundId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("HuntingGroundId");

                    b.HasIndex("UserId");

                    b.ToTable("DamageReports");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.HuntingGround", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PolygonCoordinatesJson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("TotalAreaInHectares")
                        .HasColumnType("REAL");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("HuntingGrounds");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.Registration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AnimalName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<double>("Latitude")
                        .HasColumnType("REAL");

                    b.Property<double>("Longitude")
                        .HasColumnType("REAL");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Registrations");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("LinkedHunterId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LinkedHunterId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.ChatMessage", b =>
                {
                    b.HasOne("FaunaConnect2.Api.Models.User", "Receiver")
                        .WithMany()
                        .HasForeignKey("ReceiverId");

                    b.HasOne("FaunaConnect2.Api.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Receiver");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.DamageReport", b =>
                {
                    b.HasOne("FaunaConnect2.Api.Models.HuntingGround", "HuntingGround")
                        .WithMany()
                        .HasForeignKey("HuntingGroundId");

                    b.HasOne("FaunaConnect2.Api.Models.User", "User")
                        .WithMany("DamageReports")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("HuntingGround");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.HuntingGround", b =>
                {
                    b.HasOne("FaunaConnect2.Api.Models.User", "User")
                        .WithMany("HuntingGrounds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.Registration", b =>
                {
                    b.HasOne("FaunaConnect2.Api.Models.User", "User")
                        .WithMany("Registrations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.User", b =>
                {
                    b.HasOne("FaunaConnect2.Api.Models.User", "LinkedHunter")
                        .WithMany("LinkedFarmers")
                        .HasForeignKey("LinkedHunterId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("LinkedHunter");
                });

            modelBuilder.Entity("FaunaConnect2.Api.Models.User", b =>
                {
                    b.Navigation("DamageReports");

                    b.Navigation("HuntingGrounds");

                    b.Navigation("LinkedFarmers");

                    b.Navigation("Registrations");
                });
#pragma warning restore 612, 618
        }
    }
}
