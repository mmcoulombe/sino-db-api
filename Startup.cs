using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using SinoDbAPI.Jwt;
using SinoDbAPI.Services;
using SinoDbAPI.Settings;

namespace SinoDbAPI
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
            services.Configure<SinoDataBaseSettings>(
                Configuration.GetSection(nameof(SinoDataBaseSettings)));

            services.Configure<AuthenticationSettings>(
                Configuration.GetSection(nameof(AuthenticationSettings)));

            services.AddSingleton<ISinoDataBaseSettings>(sp =>
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SinoDataBaseSettings>>().Value);

            services.AddSingleton<IAuthenticationSettings>(sp =>
                sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthenticationSettings>>().Value);

            services.AddSingleton<IMongoClient, MongoClient>(sp =>
                new MongoClient(Configuration.GetConnectionString("MongoDB")));

            services.AddSingleton<IColosseumResultService, ColosseumResultService>();
            services.AddSingleton<IUsersService, UsersService>();

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SinoDbAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SinoDbAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
