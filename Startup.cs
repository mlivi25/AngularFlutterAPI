using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Commander.Data;
using Commander.Data.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Commander
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
            services.AddCors(options => options.AddPolicy("AllowEverything", builder => builder.WithOrigins("http://localhost:4200/")
                                                                                                .SetIsOriginAllowed((host) => true)
                                                                                                .AllowAnyMethod()
                                                                                                .AllowAnyHeader()));

            services.AddDbContext<CommanderContext>(opt => opt.UseSqlServer
            (Configuration.GetConnectionString("CommanderConnection")));

            services.AddControllers().AddNewtonsoftJson(s => 
            s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // services.AddScoped<ICommanderRepo, MockCommanderRepo>(); old Hardcoded repo
            services.AddScoped<ICommanderRepo, SqlCommanderRepo>();
            services.AddScoped<IUserRepo, SqlUserRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("AllowEverything");

            app.UseHttpsRedirection();
          
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
