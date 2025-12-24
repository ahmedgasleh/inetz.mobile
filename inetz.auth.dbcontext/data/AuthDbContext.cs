using inetz.auth.dbcontext.models;
using Microsoft.EntityFrameworkCore;
using System;

namespace inetz.auth.dbcontext.data
{
    public partial class AuthDbContext : DbContext
    {
        public AuthDbContext ()
        {
        }

        public AuthDbContext ( DbContextOptions<AuthDbContext> options )
            : base(options)
        {
        }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            modelBuilder.Entity<RefreshToken>(entity =>
            {

                modelBuilder.Entity<RefreshToken>()
                    .ToTable("RefreshToken"); // 👈 Force EF to use the singular table name
                entity
                    .HasKey(e => e.Id);


                entity.Property(e => e.CreatedUtc)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeviceId).HasMaxLength(50);
                entity.Property(e => e.ExpiresUtc).HasColumnType("datetime");
                entity.Property(e => e.RevokedUtc).HasColumnType("datetime");
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.ToTable("UserProfile");   // 👈 match the SQL table name exactly

                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .IsRequired();


                entity.Property(e => e.DeviceId)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.DeviceHash)
                  .HasMaxLength(256)
                  .IsRequired();

                entity.Property(e => e.UserPassWord)
                    .HasMaxLength(256)
                    .IsRequired();
              

                entity.Property(e => e.UserEmail)
                    .HasMaxLength(50)
                    .IsRequired();
                entity.Property(e => e.BinHash).HasMaxLength(256);
                entity.Property(e => e.BinExpiresAt).HasColumnType("datetime");
                entity.Property(e => e.LastName).HasMaxLength(32);
                entity.Property(e => e.FirstName).HasMaxLength(32);
                entity.Property(e => e.Address).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial ( ModelBuilder modelBuilder );
    }
}
