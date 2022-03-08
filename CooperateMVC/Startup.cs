using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CooperateMVC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace CooperateMVC
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
			Initialize();
			services.AddSession();
			services.AddControllersWithViews();
			
			// Add a default AuthorizeFilter to all endpoints
			//services.AddRazorPages().AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));
			//services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
				//x => x.LoginPath = "/Users/BankSignIn");

		    services.AddDbContext<CooperateMVCContext>(options =>
		            options.UseSqlServer(Configuration.GetConnectionString("CooperateMVCContext")));
		}

		public static void Initialize()
		{
			var stockPath = AppDomain.CurrentDomain.BaseDirectory;
			var jsonPath = "fir-dotnet-8146c-firebase-adminsdk-5i6y8-bed6b12344.json";
			FirebaseApp.Create(new AppOptions
			{
				Credential = GoogleCredential.FromFile(Path.Combine(stockPath, jsonPath))
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			//app.UseAuthorization();
			app.UseSession();
			//app.UseAuthentication().UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
