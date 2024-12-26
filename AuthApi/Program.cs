
using AuthApi.Db;
using Microsoft.AspNetCore.Identity;
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

            #region Identity Configuration
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(ops =>
            {
                ops.User.RequireUniqueEmail = true;

                ops.Password.RequiredLength = 8;
                ops.Password.RequireDigit = true;
                ops.Password.RequireUppercase = true;
                ops.Password.RequireNonAlphanumeric = true;

            })
            .AddEntityFrameworkStores<AuthContext>()
            .AddDefaultTokenProviders();
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
