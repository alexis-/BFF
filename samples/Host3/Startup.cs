// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Host5
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.AddBff()
                .AddServerSideSessions();
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                    options.DefaultSignOutScheme = "oidc";
                })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "__Host-spa3";
                    options.Cookie.SameSite = SameSiteMode.Strict;
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://localhost:5001";
                    options.ClientId = "spa";
                    options.ClientSecret = "secret";
                    options.ResponseType = "code";
                    options.ResponseMode = "query";

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("api");
                    options.Scope.Add("offline_access");
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseRouting();

            app.UseBff();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization()
                    .AsLocalBffApiEndpoint();
                
                endpoints.MapBffManagementEndpoints();

                endpoints.MapRemoteBffApiEndpoint("/api", "https://localhost:5010")
                    .RequireAccessToken();
            });
        }
    }
}