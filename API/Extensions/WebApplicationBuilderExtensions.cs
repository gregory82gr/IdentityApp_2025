using API.Data;
using API.Models;
using API.Services;
using API.Services.IServices;
using API.Utility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<ITokenService, TokenService>();

            return builder;
        }

        public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = SD.RequiredPasswordLength;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
               
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount=true;

                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.MaxFailedAccessAttempts = SD.MaxFailedAccessAttempts;
                options.Lockout.DefaultLockoutTimeSpan =TimeSpan.FromDays(SD.DefaultLockoutTimeSpanIndays);
            })
                .AddEntityFrameworkStores<Context>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddCookie(opt =>
            {
                opt.Cookie.Name = SD.IdentityAppCookie;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuer =true,
                    
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies[SD.IdentityAppCookie];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }


    }
}
