using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserMgmt.Core.Models;

namespace UserMgmt.API
{
    public class UserMgmtDbContext : DbContext
    {
            public UserMgmtDbContext(DbContextOptions<UserMgmtDbContext> options)
                : base(options)
            {
            }

        public DbSet<User> Users { get; set; }

        public DbSet<UserRelationship> UserRelationships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
              .ToTable("Users")
              .HasDiscriminator<string>("UserType")
              .HasValue<User>("User")
              .HasValue<Manager>("Manager")
              .HasValue<Client>("Client");

            modelBuilder.Entity<UserRelationship>()
                .HasKey(ur => ur.UserRelationshipId);

            modelBuilder.Entity<UserRelationship>()
                .HasOne(ur => ur.Client)
                .WithMany(u => u.ClientRelationships)
                .HasForeignKey(ur => ur.ClientId);

            modelBuilder.Entity<UserRelationship>()
                .HasOne(ur => ur.Manager)
                .WithMany(u => u.ManagerRelationships)
                .HasForeignKey(ur => ur.ManagerId);

        }
    }
}
