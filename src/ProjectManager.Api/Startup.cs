using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using ProjectManager.Data.Entities;

namespace ProjectManager.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _enviroment;
        public Startup(IConfiguration configuration, IWebHostEnvironment enviroment)
        {
            _configuration = configuration;
            _enviroment = enviroment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString, builder =>
                {
                    builder.UseNodaTime();
                });
            });

            services.AddSingleton<IClock>(SystemClock.Instance);

            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

        }

        public void Configure(IApplicationBuilder app)
        {
            // Configure the HTTP request pipeline.
            if (_enviroment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
