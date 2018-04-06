using Microsoft.EntityFrameworkCore;

namespace Polly.Wants.A.Cracker.WebApi.Data
{
  public class ApiContext : DbContext
  {
    public ApiContext(DbContextOptions<ApiContext> options)
      : base(options)
    {
      
    }

    public DbSet<Student> Students { get; set; }
  }
}