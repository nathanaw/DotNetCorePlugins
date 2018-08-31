using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using McMaster.NETCore.Plugins;
using MefWebApp.Mef;
using MefWebApp.Pipelines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Plugin.Abstractions;

namespace MefWebApp
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; private set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Load the plugins into the MEF container so that we can build our Piplelines.
            ConfigurePipelines(services);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }



        /// <summary>
        /// Load the plugins into the MEF container so that we can
        /// build our Piplelines, and put them in the IoC container.
        /// </summary>
        /// <param name="services">The services collection of the IoC container.</param>
        public void ConfigurePipelines(IServiceCollection services)
        {
            var pluginCatalog = LoadPlugins();
            var localCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());
            var catalog = new AggregateCatalog(localCatalog, pluginCatalog);
            var container = new CompositionContainer(catalog);

            services.AddSingleton(container);
            services.AddSingleton<MessagePipeline>(sp =>
            {
                return container.GetExportedValue<MessagePipeline>();
            });
        }

        /// <summary>
        /// Loads the MEF plugin catalog for use by the composition container.
        /// </summary>
        /// <returns>A MEF catalog with all the located MEF exports from the plugins.</returns>
        public ComposablePartCatalog LoadPlugins()
        {
            // These shared types need to include any interfaces or types that the host and plugins share.
            var sharedTypes = new[]
                    {
                        typeof(IPipelineFilterStep<>),
                        typeof(IPipelineFilterStepMetadata),
                        typeof(IMessageFilterStep),
                    };

            // Search the specified directory for plugins.
            return PluginCatalog.Create(AppContext.BaseDirectory, sharedTypes);
        }

    }
}
