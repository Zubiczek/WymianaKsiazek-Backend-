using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WymianaKsiazek.Api.Database;

namespace WymianaKsiazek.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("")]
    public class BookController : BaseController
    {
        private readonly Context _context;

        public BookController(Context context)
        {
            _context = context;
        }

        [HttpGet("books")]
        public async Task<IActionResult> Get()
        {
            var books = await _context.Books.Where(x => x.UserId == CurrentUserId).ToListAsync();

            if (books.Count == 0)
                return NotFound();

            return Ok(books);
        }

        [HttpPost("books")]
        public async Task<IActionResult> Add([FromBody] string name)
        {
            _context.Books.Add(new BookEntity() { Name = name, UserId = CurrentUserId });
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
