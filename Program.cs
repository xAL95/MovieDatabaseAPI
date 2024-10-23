using LiteDB;
using Microsoft.OpenApi.Models;

namespace MovieDatabaseAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // LiteDB
            builder.Services.AddSingleton<ILiteDatabase>(new LiteDatabase("Filename=MovieDatabase.db; Connection=shared"));

            builder.Services.AddControllers();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => 
            { 
                options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1" });

                // Add security definition for API key
                options.AddSecurityDefinition("key", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter your API key",
                    Name = "key",
                    Type = SecuritySchemeType.ApiKey
                });

                // Add security requirement
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "key"
                            }
                        },
                        new string[] {}
                    }
                });

            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Middleware API-KEY
            app.UseMiddleware<Middleware.ApiKey>();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
