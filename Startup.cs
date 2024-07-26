using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Subscriptions.Example.Chat
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
            services.AddSingleton<IChatRepository, ChatRepository>();

            services.AddScoped<ISessionAccessor, SessionAccessor>();
            services.AddScoped<IChatService, ChatService>();

            services.AddSingleton<ChatTopic>();

            services.AddSingleton<IChatMessageProcessor, ChatMessageProcessor>();

            services.AddHostedService<ProcessingWorker>();

            services
                .AddGraphQLServer()
                .AddInMemorySubscriptions()
                .AddChatTypes()
                .AddGlobalObjectIdentification()
                .AddMutationConventions()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddSubscriptionType<Subscription>()
                .AddInterfaceType<IMessage>()
                .AddType<DocumentMessage>()
                .AddType<ChatMessage>()
                .AddType<ChatMessageCreated>()
                .AddType<ChatMessageUpdated>()
                .AddTypeExtension<ChatMessageCreatedExtensions>()
                .AddTypeExtension<ChatMessageUpdatedExtensions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapGraphQL(); });
        }
    }
}
