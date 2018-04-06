using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly.Wants.A.Cracker.WebApi.Data;

namespace Polly.Wants.A.Cracker.WebApi.Controllers
{
  [Route("api/[controller]")]
  public class StudentsController : Controller
  {
    private readonly ApiContext _context;

    public StudentsController(ApiContext context)
    {
      _context = context;
    }

    // GET api/values
    [HttpGet]
    public async Task<IActionResult> Get()
    {
      // trivially get some data into the in-memory database.
      Startup.AddTestData(_context);

      var items = await _context.Students.ToListAsync();
      return Ok(items);
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
      // trivially get some data into the in-memory database.
      Startup.AddTestData(_context);

      var item = await _context.Students.FirstAsync(x => x.StudentId == id);

      if (item == null)
      {
        return NotFound();
      }

      return Ok(item);
    }

    //// POST api/values
    //[HttpPost]
    //public void Post([FromBody]Student value)
    //{
    //}

    //// PUT api/values/5
    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody]Student value)
    //{
    //}

    //// DELETE api/values/5
    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
  }
}
