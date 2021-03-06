﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using TravelBlog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace TravelBlog
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        public async void DataInitialize()
        {
            TravelBlogDbContext context = new TravelBlogDbContext();

            string[] roles = new string[] { "Admin" };
            var roleStore = new RoleStore<IdentityRole>(context);
            foreach (string role in roles)
            {              if (!context.Roles.Any(r => r.Name == role))
                {
                    IdentityRole newRole = new IdentityRole(role);
                    newRole.NormalizedName = "ADMIN";
                    await roleStore.CreateAsync(newRole);
                }
            }
            var userStore = new UserStore<ApplicationUser>(context);
            if (!context.Users.Any(u => u.UserName == "parajulibhawani@gmail.com"))
            {
                var user = new ApplicationUser { UserName = "parajulibhawani@gmail.com" };
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "Test1234!");
                user.PasswordHash = hashed;

                var result = await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "ADMIN");
            }

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddEntityFramework()
                .AddDbContext<TravelBlogDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<TravelBlogDbContext>()
                .AddDefaultTokenProviders();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            DataInitialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
