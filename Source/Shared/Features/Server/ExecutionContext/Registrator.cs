﻿using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.BuildingBlocks;

namespace Shared.Features.Server.ExecutionContext
{
    public static class Registrator
    {
        public static IServiceCollection AddServerExecutionContext(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ServerExecutionContextMiddleware>();
            services.AddScoped<IExecutionContext, ServerExecutionContext>(ServerExecutionContext.CreateInstance);
            return services;
        }
    }
}
