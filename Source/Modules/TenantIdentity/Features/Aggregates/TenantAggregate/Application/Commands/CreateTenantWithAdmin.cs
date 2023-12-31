﻿using Modules.TenantIdentity.Features.Aggregates.TenantAggregate.Domain;
using Modules.TenantIdentity.Features.Infrastructure.EFCore;
using Modules.TenantIdentity.Web.Shared.DTOs.Aggregates.Tenant;
using Shared.Features.CQRS.Command;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modules.TenantIdentity.Features.Aggregates.TenantAggregate.Application.Commands
{
    public class CreateTenantWithAdmin : ICommand<TenantDTO>
    {
        public string Name { get; set; }
        public Guid AdminId { get; set; }
    }

    public class CreateTenantWithAdminCommandHandler : ICommandHandler<CreateTenantWithAdmin, TenantDTO>
    {
        private readonly TenantIdentityDbContext tenantIdentityDbContext;
        public CreateTenantWithAdminCommandHandler(TenantIdentityDbContext tenantIdentityDbContext)
        {
            this.tenantIdentityDbContext = tenantIdentityDbContext;
        }

        public async Task<TenantDTO> HandleAsync(CreateTenantWithAdmin createTenant, CancellationToken cancellationToken)
        {
            var tenant = await Tenant.CreateTenantWithAdminAsync(createTenant.Name, Guid.Empty);

            tenantIdentityDbContext.Tenants.Add(tenant);
            await tenantIdentityDbContext.SaveChangesAsync(cancellationToken);

            return tenant.ToDTO();
        }
    }
}
