using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pustok.Contracts;
using Pustok.Database.Models;
using System.Reflection.Emit;

namespace Pustok.Database.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .ToTable("Users");

            builder
               .HasOne<UserActivation>(u => u.Activation)
               .WithOne(ua => ua.User)
               .HasForeignKey<UserActivation>(ua => ua.UserId);

            builder
                .HasData(
                new User
                {
                    Id = -1,
                    Name = "Eshqin",
                    LastName = "Heyder",
                    Email = "super_admin@gmail.com",
                    Password = "$2a$11$KvzR3ws3U0AdBXgx3kGu3evEdUYqKQDIfjVbnWeh/Ze1UsqU23sOa",
                    Role = Role.Values.SuperAdmin,
                    UpdatedAt = new DateTime(2023, 09, 06, 0, 0, 0, DateTimeKind.Utc),
                    CreatedAt = new DateTime(2023, 09, 06, 0, 0, 0, DateTimeKind.Utc)
                });
        }
    }
}
