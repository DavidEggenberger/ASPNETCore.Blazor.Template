﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Features.Messaging;
using Shared.Features.EFCore;
using Shared.Features.EmailSender;
using Shared.Features.SignalR;
using Shared.Features.Misc.ExecutionContext;
using Shared.Features.Misc.Modules;

namespace Shared.Features
{
    public static class Registrator
    {
        public static IServiceCollection AddSharedFeatures(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            services.AddServerExecutionContext();
            services.AddMessaging();
            services.AddEFCore(configuration);
            services.AddEmailSender(configuration);
            services.Add_SignalR();

            return services;
        }

        public static IApplicationBuilder UseSharedFeaturesMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseEFCoreMiddleware();
            app.UseServerExecutionContextMiddleware();
            app.UseModulesMiddleware(env);
            app.UseSignalRMiddleware();

            return app;
        }
    }
}
