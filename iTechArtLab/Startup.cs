using DataAccessLayer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using DataAccessLayer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BuisnessLayer.JWToken;
using BuisnessLayer.Senders;
using BuisnessLayer.Cloudinary;
using DataAccessLayer.Entities;
using System.IO;
using BuisnessLayer;
using iTechArtLab.ActionFilters;

namespace iTechArtLab
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.Configure<JWTokenConfig>(Configuration.GetSection("JWTokenConfig"));
            services.Configure<SmtpConfig>(Configuration.GetSection("SmtpConfig"));
            services.Configure<CloudinaryConfig>(Configuration.GetSection("CloudinaryConfig"));

            services.AddTransient<SessionManager>();
            services.AddTransient<JWTokenValidator>();
            services.AddTransient<JWTokenGenerator>();
            services.AddTransient<AccessControlManager>();
            services.AddTransient<ModelValidator>();
            services.AddTransient<ProductsManager>();
            services.AddTransient<OrdersManager>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // validation of issuer with validation of token
                            ValidateIssuer = true,
                            ValidIssuer = Configuration.GetSection("JWTokenConfig").GetSection("Issuer").Value,

                            // validation of token consumer
                            ValidateAudience = true,
                            // token consumer
                            ValidAudience = Configuration.GetSection("JWTokenConfig").GetSection("Audience").Value,
                            ValidateLifetime = true,

                            // sequrity key setting
                            IssuerSigningKey = (new JWTokenConfig())
                                .GetSymmetricSecurityKey(Configuration.GetSection("JWTokenConfig").GetSection("Key").Value),
                            // sequrity key validation
                            ValidateIssuerSigningKey = true,
                        };
                    });

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(System.Convert.ToInt32(Configuration.GetSection("JWTokenConfig").GetSection("Lifetime").Value));
            });

            services.AddScoped<SearchParamsFilterAsync>();

            services.AddControllersWithViews();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ITechArtLab",
                    Description = "API for an e-commerce application",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Pavel Rafeyeu",
                        Email = string.Empty,
                        Url = new Uri("https://examplePage.by"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Some license",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "iTechArtLab.xml"));
                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "DataAccessLayer.xml"));
                c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "BuisnessLayer.xml"));
            });

            // Registers required services for health checks
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=GetInfo}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("/hc");
                endpoints.MapSwagger();
            });

            //throw new Exception("Exception for Serialog on startup");
        }
    }
}
