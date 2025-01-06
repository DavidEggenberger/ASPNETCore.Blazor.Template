﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Shared.Kernel.BuildingBlocks.Auth.Constants;
using Modules.TenantIdentity.Features.DomainFeatures.Users;
using Shared.Features.Server;
using Modules.TenantIdentity.Features.DomainFeatures.Tenants.Application.Queries;
using Modules.TenantIdentity.Features.DomainFeatures.Tenants.Application.Commands;
using Modules.TenantIdentity.Features.DomainFeatures.Users.Application.Queries;
using Modules.TenantIdentity.Public.DTOs.Tenant.Operations;
using Modules.TenantIdentity.Public.DTOs.Tenant;

namespace Modules.TenantIdentity.Web.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = PolicyConstants.TenantAdminPolicy)]
    [ApiController]
    public class TenantsController : BaseController
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        public TenantsController(SignInManager<ApplicationUser> signInManager, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.signInManager = signInManager;
        }

        [HttpGet("{tenantId}")]
        public async Task<ActionResult<TenantDTO>> GetTenant([FromRoute] Guid tenantId)
        {
            var tenant = await queryDispatcher.DispatchAsync<GetTenant, TenantDTO>(new GetTenant { TenantId = tenantId });

            return Ok(tenant);
        }

        [HttpPost]
        public async Task<ActionResult<TenantDTO>> CreateTenant(CreateTenantDTO createTenantDTO)
        {
            validationService.ThrowIfInvalidModel(createTenantDTO);

            var createTenant = new CreateTenant
            {
                AdminId = executionContext.UserId,
                Name = createTenantDTO.Name
            };
            var createdTenant = await commandDispatcher.DispatchAsync<CreateTenant, TenantDTO>(createTenant);

            var user = await queryDispatcher.DispatchAsync<GetExecutingUser, ApplicationUser>(new GetExecutingUser { ExecutingUserId = executionContext.UserId });
            await signInManager.RefreshSignInAsync(user);
            
            return CreatedAtAction(nameof(CreateTenant), createdTenant);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTenant([FromRoute] Guid id)
        {
            await commandDispatcher.DispatchAsync<DeleteTenant>(new DeleteTenant { ExecutingUserId = executionContext.UserId, TenantId = id });

            return Ok();
        }

        [HttpPost("{tenantId}/memberships")]
        public async Task<ActionResult> AddMember([FromRoute] Guid tenantId, InviteUserToTenantDTO inviteUserToGroupDTO)
        {
            var addMember = new AddMemberToTenant
            {
                ExecutingUserId = executionContext.UserId,
                
            };

            await commandDispatcher.DispatchAsync<AddMemberToTenant>(null);
            
            return Ok();
        }

        [HttpPut("{tenantId}/memberships")]
        public async Task<ActionResult> UpdateTenantMembership([FromRoute] Guid tenantId, ChangeRoleOfTenantMemberDTO changeRoleOfTeamMemberDTO)
        {
            var updateRoleOfMemberInTenant = new UpdateRoleOfMemberInTenant
            {

            };

            await commandDispatcher.DispatchAsync<UpdateRoleOfMemberInTenant>(updateRoleOfMemberInTenant);

            return Ok();
        }

        [HttpDelete("{tenantId}/memberships/{userId}")]
        public async Task<ActionResult> RemoveMember([FromRoute] Guid tenantId, [FromRoute] Guid userId)
        {
            var removeMember = new RemoveMemberFromTenant
            {
                ExecutingUserId = executionContext.UserId,
                TenantId = tenantId,
                UserId = userId
            };

            await commandDispatcher.DispatchAsync<RemoveMemberFromTenant>(removeMember);

            return Ok();
        }
    }
}

