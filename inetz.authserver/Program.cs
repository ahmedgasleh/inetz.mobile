
using inetz.auth.dbcontext.data;
using inetz.auth.dbcontext.services;
using Microsoft.EntityFrameworkCore;
using System;

namespace inetz.authserver
{
    public class Program
    {
        public static void Main ( string [] args )
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("AuthServerConnection");

            builder.Services.AddDbContext<AuthDbContext>(option =>
            option
            .UseSqlServer(connectionString)

            );

            // Services
            builder.Services.AddScoped<ITokenService, TokenService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
