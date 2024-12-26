
using AuthApi.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            #region JWT Configuration
            builder.Services.AddAuthentication(ops =>
            {
                ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(ops =>
            {
                ops.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
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
