using BeehiveAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace BeehiveAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure the database
            builder.Services.AddDbContext<BeehiveContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BeehiveContext")));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();
            app.UseCors("AllowAll");

            // Create and seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<BeehiveContext>();
                    context.Database.EnsureCreated();
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.Run();


        }
    }

    public static class DbInitializer
    {
        public static void Initialize(BeehiveContext context)
        {
            context.Database.EnsureCreated();

            // Check if the database already has data
            if (context.Beehives.Any())
            {
                return;   // Database has been seeded
            }

            var beehives = new BeehiveAPI.Models.Beehive[]
            {
                new BeehiveAPI.Models.Beehive { Location = "Orchard", CreatedAt = DateTime.Now.AddDays(-30) },
                new BeehiveAPI.Models.Beehive { Location = "Meadow", CreatedAt = DateTime.Now.AddDays(-20) },
                new BeehiveAPI.Models.Beehive { Location = "Forest Edge", CreatedAt = DateTime.Now.AddDays(-10) }
            };

            foreach (var beehive in beehives)
            {
                context.Beehives.Add(beehive);
            }
            context.SaveChanges();

            var sensors = new BeehiveAPI.Models.Sensor[]
            {
                new BeehiveAPI.Models.Sensor { BeehiveId = 1, SensorType = "Temperature", Value = 25.5f, RecordedAt = DateTime.Now.AddHours(-2) },
                new BeehiveAPI.Models.Sensor { BeehiveId = 1, SensorType = "Humidity", Value = 60.2f, RecordedAt = DateTime.Now.AddHours(-2) },
                new BeehiveAPI.Models.Sensor { BeehiveId = 2, SensorType = "Temperature", Value = 26.1f, RecordedAt = DateTime.Now.AddHours(-1) },
                new BeehiveAPI.Models.Sensor { BeehiveId = 3, SensorType = "Weight", Value = 45.3f, RecordedAt = DateTime.Now }
            };

            foreach (var sensor in sensors)
            {
                context.Sensors.Add(sensor);
            }
            context.SaveChanges();
        }
    }




}

