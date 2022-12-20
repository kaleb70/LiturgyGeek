using LiturgyGeek.Api.Data;
using LiturgyGeek.Framework.Calendars;

namespace LiturgyGeek.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSingleton<IChurchCalendarProvider, ChurchCalendarProvider>();
            builder.Services.AddSingleton<CalendarEvaluator>();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                                      {
                                          policy.WithOrigins("https://localhost:4200");
                                      });
            });

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