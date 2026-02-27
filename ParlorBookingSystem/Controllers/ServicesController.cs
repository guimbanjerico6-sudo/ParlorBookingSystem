using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParlorBookingSystem.Models;

namespace ParlorBookingSystem.Controllers
{
    // 1. THE ROUTING: This tells the internet how to reach this Waiter
    // "api/[controller]" automatically translates to the URL: /api/services
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // 2. DEPENDENCY INJECTION: Handing the Waiter the keys to the kitchen
        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 3. THE ENDPOINT: What happens when a customer asks for the menu?
        // GET: api/services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            // The Waiter asks the database for the list of Services
            var services = await _context.Services.ToListAsync();

            // The Waiter hands the list back to the customer with an HTTP 200 (OK) status
            return Ok(services);
        }
    }
}