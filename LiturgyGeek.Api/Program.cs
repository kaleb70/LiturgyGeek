using LiturgyGeek.Calendars;
using LiturgyGeek.Calendars.Engine;
using LiturgyGeek.Data;
using Microsoft.EntityFrameworkCore;

namespace LiturgyGeek.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Configuration.AddEnvironmentVariables("LiturgyGeek_");

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<CalendarManager>();

            builder.Services.AddDbContext<LiturgyGeekContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString(builder.Environment.EnvironmentName)));

#if DEBUG
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                                      {
                                          policy.WithOrigins("https://localhost:4200");
                                      });
            });
#endif // DEBUG

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}