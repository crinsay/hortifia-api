using Hortifia.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hortifia.Infrastructure.Persistence.EntityConfigurations;

internal class UserDeviceTokenConfiguration : IEntityTypeConfiguration<UserDeviceToken>
{
    public void Configure(EntityTypeBuilder<UserDeviceToken> builder)
    {
        builder.HasKey(udt => new { udt.UserId, udt.DeviceToken });

        builder.Property(udt => udt.DeviceToken)
            .HasMaxLength(255);

        builder.HasOne(udt => udt.User)
            .WithMany()
            .HasForeignKey(udt => udt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}