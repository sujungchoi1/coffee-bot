﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with EchoBot .NET Template version v4.14.1

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CoffeeBot.Bots;
using CoffeeBot.Dialogs;

namespace EchoBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state.
            // var storage = new MemoryStorage();
            // // Create the User state passing in the storage layer.
            // var userState = new UserState(storage);
            // services.AddSingleton(userState);

            // // Create the Conversation state passing in the storage layer.
            // var conversationState = new ConversationState(storage);
            // services.AddSingleton(conversationState);

            // Create the storage we'll be using for User and Conversation state
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state.
            services.AddSingleton<UserState>();

            // Create the Conversation state.
            services.AddSingleton<ConversationState>();

            // The Dialog that will be run by the bot.
            services.AddSingleton<CoffeeDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            // services.AddTransient<IBot, Bots.EchoBot>();
            services.AddTransient<IBot, CoffeeOrderBot<CoffeeDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
