﻿using Modules.TenantIdentity.Features.Infrastructure.EFCore;
using Modules.TenantIdentity.Web.Shared.DTOs.Tenant;
using Shared.Features.Messaging.Query;
using System.Threading;

namespace Modules.TenantIdentity.Features.DomainFeatures.TenantAggregate.Application.Queries
{
    public class GetTenantByID : IQuery<TenantDTO>
    {
        public Guid TenantId { get; set; }
    }
    public class GetTenantByIdQueryHandler : IQueryHandler<GetTenantByID, TenantDTO>
    {
        private readonly TenantIdentityDbContext tenantIdentityDbContext;
        public GetTenantByIdQueryHandler(TenantIdentityDbContext tenantIdentityDbContext)
        {
            this.tenantIdentityDbContext = tenantIdentityDbContext;
        }

        public async Task<TenantDTO> HandleAsync(GetTenantByID query, CancellationToken cancellation)
        {
            var tenant = await tenantIdentityDbContext.GetTenantByIdAsync(query.TenantId);
            return tenant.ToDTO();
        }
    }
}
