using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.Wants.A.Cracker.WebApi.Data;

namespace Polly.Wants.A.Cracker.WebApi
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
      services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("Education"));
      services.AddMvc();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseMvc();
    }

    public static void AddTestData(ApiContext context)
    {
      var student = new Student
      {
        
        FirstName = "Luke",
        LastName = "Skywalker",
        DateOfBirth = new DateTime(2002,01,01).ToUniversalTime()
      };

      var student2 = new Student
      {

        FirstName = "Leia",
        LastName = "Skywalker",
        DateOfBirth = new DateTime(2002, 01, 01).ToUniversalTime()
      };


      context.Students.Add(student);
      context.Students.Add(student2);

      context.SaveChanges();
    }
  }
}
