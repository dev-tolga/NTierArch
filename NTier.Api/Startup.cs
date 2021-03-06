using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NTier.Api.Infrastructure;
using NTier.Core.Caching;
using NTier.Core.UnitOfWork;
using NTier.Data;
using NTier.Data.Entities;
using NTier.Service.BlogService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NTier.Api
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

            services.AddControllers();

            //DbContext
            services.AddDbContext<NTierDBContext>().AddUnitOfWork<NTierDBContext>(Configuration);

            services.AddDbContext<NTierDBContext>(options =>
            options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("NTier.Data")));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            //InMemeoryCache
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddTransient<IBlogService, BlogService>();
            services.AddTransient<IRedisCacheService, RedisCacheService>();
            //Automapper
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NTier.Api", Version = "v1" });
            });

            //services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            //{
            //    builder.WithOrigins("*")
            //    .AllowAnyMethod()
            //    .AllowAnyHeader()
            //    .Build();
            //}));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NTier.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
