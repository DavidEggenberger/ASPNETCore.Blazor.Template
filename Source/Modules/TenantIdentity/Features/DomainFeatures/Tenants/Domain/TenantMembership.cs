﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Shared.Features.Domain;
using Shared.Kernel.BuildingBlocks.Auth;
using Modules.TenantIdentity.Shared.DTOs.Tenant;

namespace Modules.TenantIdentity.Features.DomainFeatures.Tenants.Domain
{
    public class TenantMembership : Entity
    {
        private TenantMembership() { }
        public TenantMembership(Guid userId, TenantRole role)
        {
            UserId = userId;
            Role = role;
        }

        public Guid UserId { get; private set; }
        public Tenant Tenant { get; private set; }
        public TenantRole Role { get; private set; }

        public void UpdateRole(TenantRole role)
        {
            Role = role;
        }

        public TenantMembershipDTO ToDTO()
        {
            return new TenantMembershipDTO()
            {
                UserId = UserId,
                TenantId = Tenant.Id,
                Role = Role
            };
        }
    }

    public class TenantMembershipEFConfiguration : IEntityTypeConfiguration<TenantMembership>
    {
        public void Configure(EntityTypeBuilder<TenantMembership> builder)
        {
            builder.ToTable("TenantMembership");
        }
    }
}
