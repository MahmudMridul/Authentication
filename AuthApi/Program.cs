
using AuthApi.Db;
using Microsoft.EntityFrameworkCore;

namespace AuthApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Controller Configuration
            builder.Services.AddControllers();
            #endregion

            #region Database Configuration
            builder.Services.AddDbContext<AuthContext>(ops =>
            {
                ops.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });
            #endregion


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
