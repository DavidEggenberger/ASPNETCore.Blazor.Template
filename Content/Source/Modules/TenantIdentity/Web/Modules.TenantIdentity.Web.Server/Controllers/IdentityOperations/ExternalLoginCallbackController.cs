﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modules.TenantIdentity.DomainFeatures.UserAggregate.Application.Commands;
using Modules.TenantIdentity.DomainFeatures.UserAggregate.Application.Queries;
using Modules.TenantIdentity.DomainFeatures.UserAggregate.Domain;
using Shared.Kernel.BuildingBlocks.Authorization.Constants;
using Shared.Kernel.Extensions;
using Shared.Web.Server;
using System;
using System.Threading.Tasks;

namespace Modules.TenantIdentity.Web.Server.Controllers.IdentityOperations
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class ExternalLoginCallbackController : BaseController
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        public ExternalLoginCallbackController(SignInManager<User> signInManager, UserManager<User> userManager, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await signInManager.GetExternalLoginInfoAsync();

            var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (info is not null && user is null)
            {
                User _user = new User
                {
                    UserName = info.Principal.Identity.Name,
                    Email = info.Principal.GetClaimValue(ClaimConstants.EmailClaimType),
                    PictureUri = info.Principal.GetClaimValue(ClaimConstants.PictureClaimType)
                };

                var createUserCommand = new CreateUserCommand { User = _user };
                await commandDispatcher.DispatchAsync(createUserCommand);
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: false);
            return signInResult switch
            {
                Microsoft.AspNetCore.Identity.SignInResult { Succeeded: true } => LocalRedirect("/"),
                Microsoft.AspNetCore.Identity.SignInResult { RequiresTwoFactor: true } => RedirectToPage("/TwoFactorLogin", new { ReturnUrl = returnUrl }),
                _ => LocalRedirect("/")
            };
        }
    }
}
